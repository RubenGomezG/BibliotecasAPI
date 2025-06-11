using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V2;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V2
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v2/autores")]    
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServicioAutoresV2 _servicioAutoresV2;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CACHE_AUTORES = "autores-obtener";


        public AutoresController(ApplicationDbContext context, IMapper mapper,
             IServicioAutoresV2 servicioAutoresV2,
             IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _mapper = mapper;
            _servicioAutoresV2 = servicioAutoresV2;
            _outputCacheStore = outputCacheStore;
        }
        
        [HttpGet] // api/autores
        [AllowAnonymous]        
        public async Task<IEnumerable<AutorDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await _servicioAutoresV2.GetAutores(paginacionDTO);
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult> Filtrar([FromQuery] AutorFiltroDTO autorFiltroDTO)
        {
            var autores = await _servicioAutoresV2.Filtrar(autorFiltroDTO);
            return Ok(autores);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutorV2")] // api/autores/id
        [EndpointSummary("Obtiene autor por id")]
        [EndpointDescription("Obtiene autor por id. Incluye sus libros. Si el autor no existe, se retorna 404")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<AutorConLibrosDTO>> Get(int id, bool incluirLibros = false)
        {
            return await _servicioAutoresV2.GetAutorPorIdV2(id, incluirLibros);
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            return await _servicioAutoresV2.AnadirAutor(autorCreacionDTO);
        }

        [HttpPost("autor-con-foto")]
        public async Task<ActionResult> PostConFoto([FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _servicioAutoresV2.AnadirAutorConFoto(autorCreacionDTO);
        }

        [HttpPut("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, [FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _servicioAutoresV2.ActualizarAutor(id, autorCreacionDTO);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }

            var autorDB = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);

            if (autorDB is null)
            {
                return NotFound();
            }

            var autorPatchDTO = _mapper.Map<AutorPatchDTO>(autorDB);
            patchDoc.ApplyTo(autorPatchDTO, ModelState);

            if (!TryValidateModel(autorPatchDTO))
            {
                return ValidationProblem();
            }

            _mapper.Map(autorPatchDTO, autorDB);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);
            return new NoContentResult();
        }

        [HttpDelete("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioAutoresV2.BorrarAutor(id);
        }
    }
}
