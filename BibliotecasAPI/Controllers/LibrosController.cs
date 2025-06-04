using AutoMapper;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BibliotecasAPI.Controllers
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/libros")]
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
          
        [HttpGet("{id:int}", Name = "ObtenerLibro")] //api/libros/{id}
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
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }

            var autoresIdsExisten = await _context.Autores
                                    .Where(a => libroCreacionDTO.AutoresIds.Contains(a.Id))
                                    .Select(x => x.Id)
                                    .ToListAsync();

            if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                var autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), $"Los siguientes autores {string.Join(',', autoresNoExisten)} no existen.");
                return ValidationProblem();
            }

            var libro = _mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);

            _context.Add(libro);
            await _context.SaveChangesAsync();

            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);
            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libro);
        }

        

        [HttpPut("{id:int}")] //Put/api/libros/{id}
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }

            var autoresIdsExisten = await _context.Autores
                                    .Where(a => libroCreacionDTO.AutoresIds.Contains(a.Id))
                                    .Select(x => x.Id)
                                    .ToListAsync();

            if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                var autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), $"Los siguientes autores {string.Join(',', autoresNoExisten)} no existen.");
                return ValidationProblem();
            }

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
