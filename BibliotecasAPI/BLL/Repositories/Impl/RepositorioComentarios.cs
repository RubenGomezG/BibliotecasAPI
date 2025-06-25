using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils;
using BibliotecasAPI.Utils.ClassUtils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CACHE_COMENTARIOS = "comentarios-obtener";

        public RepositorioComentarios(ApplicationDbContext context, IMapper mapper, IServicioUsuarios servicioUsuarios,
            IOutputCacheStore outputCacheStore, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _servicioUsuarios = servicioUsuarios;
            _outputCacheStore = outputCacheStore;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActionResult<IEnumerable<ComentarioDTO>>> GetComentariosDeLibro(int libroId)
        {
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return new NotFoundResult();
            }
            var comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Where(c => c.LibroId == libroId)
                .OrderByDescending(l => l.FechaPublicacion)
                .ToListAsync();

            var comentariosDTO = _mapper.Map<List<ComentarioDTO>>(comentarios);
            return comentariosDTO;
        }

        public async Task<ActionResult<ComentarioConLibroDTO>> GetComentarioPorId(Guid id)
        {
            var comentario = await _context.Comentarios
                                    .Include(c => c.Usuario)
                                    .Include(c => c.Libro)
                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (ComentarioUtils.ExisteComentario(comentario))
            {
                return new NotFoundResult();
            }

            var comentarioDTO = _mapper.Map<ComentarioConLibroDTO>(comentario);
            return comentarioDTO;
        }

        public async Task<ActionResult> AnadirComentario(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return new NotFoundResult();
            }

            var usuario = await _servicioUsuarios.ObtenerUsuario();
            if (UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }

            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId = usuario!.Id;
            _context.Add(comentario);
            await _context.SaveChangesAsync();

            await _outputCacheStore.EvictByTagAsync(CACHE_COMENTARIOS, default);
            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);
            return new CreatedAtRouteResult("ObtenerComentarioV1", new { id = comentario.Id, libroId }, comentarioDTO);
        }
        public async Task<ActionResult> ActualizarComentario(int id, AutorCreacionDTO autorCreacionDTO)
        {
            if (_context.Autores.Any(a => a.Id == id))
            {
                var autor = _mapper.Map<Autor>(autorCreacionDTO);
                autor.Id = id;
                _context.Update(autor);
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(CACHE_COMENTARIOS, default);
                return new NoContentResult();
            }
            return new NotFoundResult();
        }

        public async Task<ActionResult> PatchComentario(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return new BadRequestResult();
            }
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return new NotFoundResult();
            }

            var usuario = await _servicioUsuarios.ObtenerUsuario();
            Comentario? comentarioDB = await _context.Comentarios.FirstOrDefaultAsync(a => a.Id == id);
            ActionResult result = await ComentarioUtils.ValidarComentario(_servicioUsuarios, comentarioDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                var comentarioPatchDTO = _mapper.Map<ComentarioPatchDTO>(comentarioDB);
                Controller? controller = _httpContextAccessor.HttpContext!.GetEndpoint()!.Metadata.GetMetadata<Controller>();
                patchDoc.ApplyTo(comentarioPatchDTO, controller!.ModelState);

                if (!controller.TryValidateModel(comentarioPatchDTO))
                {
                    return controller.ValidationProblem();
                }
                _mapper.Map(comentarioPatchDTO, comentarioDB);
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(CACHE_COMENTARIOS, default);
            }

            return result;
        }

        public async Task<ActionResult> BorradoLogicoComentario(Guid id, int libroId)
        {
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return new NotFoundResult();
            }

            var comentarioDB = await _context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
            ActionResult result = await ComentarioUtils.ValidarComentario(_servicioUsuarios, comentarioDB);

            if (result.GetType() == typeof(NoContentResult))
            {
                //--------------Borrado Físico---------------
                //var registrosBorrados = await _context.Comentarios.Where(a => a.Id == id).ExecuteDeleteAsync();

                //if (registrosBorrados == 0)
                //{
                //    return NotFound();
                //}

                //--------------Borrado Lógico---------------
                comentarioDB!.Eliminado = true;
                _context.Update(comentarioDB);
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(CACHE_COMENTARIOS, default);
            }
            
            return result;
        }
    }
}
