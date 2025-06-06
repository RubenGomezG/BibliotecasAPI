using AutoMapper;
using BibliotecasAPI.BLL.Interfaces.IServices;
using BibliotecasAPI.BLL.Interfaces.IServices.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IServicioComentarios _servicioComentarios;
        private const string CACHE_COMENTARIOS = "comentarios-obtener";

        public ComentariosController(ApplicationDbContext context, IMapper mapper,
             IServicioUsuarios servicioUsuarios,
             IServicioComentarios servicioComentarios)
        {
            _context = context;
            _mapper = mapper;
            _servicioUsuarios = servicioUsuarios;
            _servicioComentarios = servicioComentarios;
        }

        [HttpGet] // api/libros/libroId/comentarios
        [AllowAnonymous]
        [OutputCache(Tags = [CACHE_COMENTARIOS])]
        public async Task<ActionResult<IEnumerable<ComentarioDTO>>> Get(int libroId)
        {
            return await _servicioComentarios.GetComentariosDeLibro(libroId);
        }

        [HttpGet("{id}", Name = "ObtenerComentarioV1")] // api/autores/id
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<ComentarioConLibroDTO>> Get(Guid id)
        {
            return await _servicioComentarios.GetComentarioPorId(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            return await _servicioComentarios.AnadirComentario(libroId, comentarioCreacionDTO);
        }

        [HttpPut("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
        {
            return await _servicioComentarios.ActualizarComentario(id, autorCreacionDTO);
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

            return await _servicioComentarios.PatchComentario(comentarioDB, comentarioPatchDTO);
        }


        [HttpDelete("{id}")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(Guid id, int libroId)
        {
            return await _servicioComentarios.BorradoLogicoComentario(id, libroId);
        }
    }
}
