using AutoMapper;
using BibliotecasAPI.BLL.IServices;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Extensions;
using BibliotecasAPI.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BibliotecasAPI.Controllers
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/autores")]
    //[Route("api/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly ILogger<AutoresController> _logger;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CONTENEDOR = "autores";
        private const string CACHE_AUTORES = "autores-obtener";

        public AutoresController(ApplicationDbContext context, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos, ILogger<AutoresController> logger,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
            this._logger = logger;
            this._outputCacheStore = outputCacheStore;
        }
        
        [HttpGet] // api/autores
        [AllowAnonymous]
        [OutputCache(Tags = [CACHE_AUTORES])]
        [ServiceFilter<MiFiltroDeAccion>()]
        public async Task<IEnumerable<AutorDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
            {
            var queryable = _context.Autores.Include(a => a.Libros).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var autores = await queryable
                            .OrderBy(a => a.Nombre)
                            .Paginar(paginacionDTO)
                            .ToListAsync();

            var autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
            return autoresDTO;
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult> Filtrar([FromQuery] AutorFiltroDTO autorFiltroDTO)
        {
            var queryable = _context.Autores.AsQueryable();

            if (!string.IsNullOrEmpty(autorFiltroDTO.Nombre))
            {
                queryable = queryable.Where(a => a.Nombre.Contains(autorFiltroDTO.Nombre));
            }

            if (!string.IsNullOrEmpty(autorFiltroDTO.Apellidos))
            {
                queryable = queryable.Where(a => a.Apellidos.Contains(autorFiltroDTO.Apellidos));
            }

            if (autorFiltroDTO.IncluirLibros)
            {
                queryable = queryable.Include(a => a.Libros).ThenInclude(al => al.Libro);
            }

            if (autorFiltroDTO.TieneFoto.HasValue)
            {
                if (autorFiltroDTO.TieneFoto.Value)
                {
                    queryable = queryable.Where(a => a.Foto != null);
                }
                else
                {
                    queryable = queryable.Where(a => a.Foto == null);
                }
            }

            if (autorFiltroDTO.TieneLibros.HasValue)
            {
                if (autorFiltroDTO.TieneLibros.Value)
                {
                    queryable = queryable.Where(a => a.Libros.Any());
                }
                else
                {
                    queryable = queryable.Where(a => !a.Libros.Any());
                }
            }

            if (!string.IsNullOrEmpty(autorFiltroDTO.TituloLibro))
            {
                queryable = queryable.Where(a => a.Libros.Any(l => l.Libro!.Titulo.Contains("rosi")));
            }

            if (!string.IsNullOrEmpty(autorFiltroDTO.CampoOrdenar))
            {
                var tipoOrden = autorFiltroDTO.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    queryable = queryable.OrderBy($"{autorFiltroDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {
                    queryable = queryable.OrderBy(a => a.Nombre);
                    _logger.LogError(ex.Message, ex);
                }
                
            }
            else
            {
                queryable = queryable.OrderBy(a => a.Nombre);
            }

            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable
                        .Paginar(autorFiltroDTO.PaginacionDTO)
                        .ToListAsync();

            if (autorFiltroDTO.IncluirLibros)
            {                            
                var autoresDTO = _mapper.Map<IEnumerable<AutorConLibrosDTO>>(autores);
                return Ok(autoresDTO);
            }
            else
            {
                var autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
                return Ok(autoresDTO);
            }
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")] // api/autores/id
        [EndpointSummary("Obtiene autor por id")]
        [EndpointDescription("Obtiene autor por id. Incluye sus libros. Si el autor no existe, se retorna 404")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<AutorConLibrosDTO>> Get(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.Libros)
                .ThenInclude(l => l.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return NotFound();
            }

            var autorDTO = _mapper.Map<AutorConLibrosDTO>(autor);
            return autorDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            _context.Add(autor);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);

            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autorDTO.Id}, autorDTO);
        }

        [HttpPost("autor-con-foto")]
        public async Task<ActionResult> PostConFoto([FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            var autor = _mapper.Map<Autor>(autorCreacionDTO);

            if (autorCreacionDTO.Foto != null)
            {
                var url = await _almacenadorArchivos.Almacenar(CONTENEDOR, autorCreacionDTO.Foto);
                autor.Foto = url;
            }

            _context.Add(autor);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);

            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autorDTO.Id}, autorDTO);
        }

        [HttpPut("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, [FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            if (await _context.Autores.AnyAsync(a => a.Id == id))
            {
                var autor = _mapper.Map<Autor>(autorCreacionDTO);
                autor.Id = id;

                if (autorCreacionDTO.Foto != null)
                {
                    var fotoActual = await _context.Autores.Where(a => a.Id == id).Select(a => a.Foto).FirstAsync();
                    var url = await _almacenadorArchivos.Editar(fotoActual, CONTENEDOR, autorCreacionDTO.Foto);
                    autor.Foto = url;
                }

                _context.Update(autor);
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);
                return NoContent();
            }
            return NotFound();
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
            return NoContent();
        }

        [HttpDelete("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if(autor is null)
            {
                return NotFound();
            }

            _context.Remove(autor);
            await _context.SaveChangesAsync();

            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);
            await _almacenadorArchivos.Borrar(autor.Foto, CONTENEDOR);
            return NoContent();
        }
    }
}
