using AutoMapper;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Extensions;
using BibliotecasAPI.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BibliotecasAPI.Controllers.V2
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v2/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CACHE_LIBROS = "libros-obtener";

        public LibrosController(ApplicationDbContext context, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _mapper = mapper;
            _outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Tags = [CACHE_LIBROS])]
        public async Task<IEnumerable<LibroConAutoresDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = _context.Libros
                            .Include(l => l.Autores)
                            .ThenInclude(a => a.Autor)
                            .AsQueryable();

            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var libros = await queryable
                            .OrderBy(a => a.Titulo)
                            .Paginar(paginacionDTO)
                            .ToListAsync();

            var librosDTO = _mapper.Map<IEnumerable<LibroConAutoresDTO>>(libros);
            return librosDTO;
        }
          
        [HttpGet("{id:int}", Name = "ObtenerLibroV2")] //api/libros/{id}
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.Autores)
                .ThenInclude(a => a.Autor)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro is null)
            {
                return NotFound();
            }

            var libroDTO = _mapper.Map<LibroConAutoresDTO>(libro);
            return libroDTO;
        }

        [HttpPost]
        [ServiceFilter<FiltroValidacionLibro>()]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            var libro = _mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);

            _context.Add(libro);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);

            var libroDTO = _mapper.Map<LibroDTO>(libro);
            //return Ok(libroDTO);
            return CreatedAtRoute("ObtenerLibroV2", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}")] //Put/api/libros/{id}
        [ServiceFilter<FiltroValidacionLibro>()]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await _context.Libros
                            .Include(l => l.Autores)
                            .FirstOrDefaultAsync(l => l.Id == id);
            
            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = _mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);
            return NoContent();
        }

        [HttpDelete("{id:int}")] //Put/api/libros/{id}
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados = await _context.Libros.Where(a => a.Id == id).ExecuteDeleteAsync();

            if (registrosBorrados == 0)
            {
                return NotFound();
            }

            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.Autores != null)
            {
                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Orden = i;
                }
            }
        }
    }
}
