using AutoMapper;
using BibliotecasAPI.BLL.IServices;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServicioUsuarios _servicioUsuarios;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, IServicioUsuarios servicioUsuarios)
        {
            this._context = context;
            this._mapper = mapper;
            this._servicioUsuarios = servicioUsuarios;
        }

        [HttpGet] // api/libros/libroId/comentarios
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ComentarioDTO>>> Get(int libroId)
        {
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return NotFound();
            }
            var comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Where(c => c.LibroId == libroId)
                .OrderByDescending(l => l.FechaPublicacion)
                .ToListAsync();
            var comentariosDTO = _mapper.Map<List<ComentarioDTO>>(comentarios);
            return comentariosDTO;
        }


        [HttpGet("{id}", Name = "ObtenerComentario")] // api/autores/id
        [AllowAnonymous]
        public async Task<ActionResult<ComentarioConLibroDTO>> Get(Guid id)
        {
            var comentario = await _context.Comentarios
                                    .Include (c => c.Usuario)
                                    .Include(c => c.Libro)
                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (comentario is null)
            {
                return NotFound();
            }

            var comentarioDTO = _mapper.Map<ComentarioConLibroDTO>(comentario);
            return comentarioDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return NotFound();
            }

            var usuario = await _servicioUsuarios.ObtenerUsuario();
            if (usuario == null)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId = usuario.Id;
            _context.Add(comentario);
            await _context.SaveChangesAsync();
            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
        {
            if (_context.Autores.Any(a => a.Id == id))
            {
                var autor = _mapper.Map<Autor>(autorCreacionDTO);
                autor.Id = id;
                _context.Update(autor);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return NotFound();
            }

            var usuario = await _servicioUsuarios.ObtenerUsuario();
            if (usuario == null)
            {
                return NotFound();
            }

            var comentarioDB = await _context.Comentarios.FirstOrDefaultAsync(a => a.Id == id);

            if (comentarioDB is null)
            {
                return NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
            }

            var comentarioPatchDTO = _mapper.Map<ComentarioPatchDTO>(comentarioDB);
            patchDoc.ApplyTo(comentarioPatchDTO, ModelState);

            if (!TryValidateModel(comentarioPatchDTO))
            {
                return ValidationProblem();
            }

            _mapper.Map(comentarioPatchDTO, comentarioDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(Guid id, int libroId)
        {
            if (!await LibroUtils.ExisteLibro(_context, libroId))
            {
                return NotFound();
            }

            var usuario = await _servicioUsuarios.ObtenerUsuario();
            if (usuario == null)
            {
                return NotFound();
            }

            var comentarioDB = await _context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);

            if (comentarioDB is null)
            {
                return NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
            }

            //var registrosBorrados = await _context.Comentarios.Where(a => a.Id == id).ExecuteDeleteAsync();

            //if (registrosBorrados == 0)
            //{
            //    return NotFound();
            //}

            comentarioDB.Eliminado = true;
            _context.Update(comentarioDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
