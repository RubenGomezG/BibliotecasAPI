using AutoMapper;
using BibliotecasAPI.Datos;
using BibliotecasAPI.DTO.LibroDTOs;
using BibliotecasAPI.Model.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        [HttpGet]
        public async Task<IEnumerable<LibroDTO>> Get()
        {
            var libros = await _context.Libros.ToListAsync();
            var librosDTO = _mapper.Map<IEnumerable<LibroDTO>>(libros);
            return librosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")] //api/libros/{id}
        public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound();
            }

            var libroDTO = _mapper.Map<LibroConAutorDTO>(libro);
            return libroDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            var libro = _mapper.Map<Libro>(libroCreacionDTO);
            var existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                ModelState.AddModelError(nameof(Libro.AutorId), $"El autor de id {libro.AutorId} no existe");
                return ValidationProblem();
            }

            _context.Add(libro);
            await _context.SaveChangesAsync();

            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libro);
        }

        [HttpPut("{id:int}")] //Put/api/libros/{id}
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libro = _mapper.Map<Libro>(libroCreacionDTO);

            if (_context.Libros.Any(a => a.Id == id) && _context.Autores.Any(x => x.Id == libro.AutorId))
            {
                _context.Update(libro);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();

        }

        [HttpDelete("{id:int}")] //Put/api/libros/{id}
        public async Task<ActionResult> Delete(int id)
        {
            //if (_context.Libros.Any(a => a.Id == id))
            //{
            //    _context.Remove(id);
            //    await _context.SaveChangesAsync();
            //    return Ok();
            //}
            //return NotFound();
            var registrosBorrados = await _context.Libros.Where(a => a.Id == id).ExecuteDeleteAsync();

            if (registrosBorrados == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
