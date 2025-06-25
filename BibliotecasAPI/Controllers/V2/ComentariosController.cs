using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils;
using BibliotecasAPI.Utils.ClassUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V2
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

        [HttpGet(Name = "ObtenerComentariosDeLibroV2")] // api/libros/libroId/comentarios
        [AllowAnonymous]
        [OutputCache(Tags = [CACHE_COMENTARIOS])]
        public async Task<ActionResult<IEnumerable<ComentarioDTO>>> Get(int libroId)
        {
            return await _servicioComentarios.GetComentariosDeLibro(libroId);
        }

        [HttpGet("{id}", Name = "ObtenerComentarioV2")] // api/autores/id
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<ComentarioConLibroDTO>> Get(Guid id)
        {
            return await _servicioComentarios.GetComentarioPorId(id);
        }

        [HttpPost(Name = "CrearComentarioV2")]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            return await _servicioComentarios.AnadirComentario(libroId, comentarioCreacionDTO);
        }

        [HttpPut("{id:int}", Name = "ActualizarComentarioV2")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
        {
            return await _servicioComentarios.ActualizarComentario(id, autorCreacionDTO);
        }

        [HttpPatch("{id}", Name = "PatchComentarioV2")]
        public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            return await _servicioComentarios.PatchComentario(id, libroId, patchDoc);
        }


        [HttpDelete("{id}", Name = "BorrarComentarioV2")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(Guid id, int libroId)
        {
            return await _servicioComentarios.BorradoLogicoComentario(id, libroId);
        }
    }
}
