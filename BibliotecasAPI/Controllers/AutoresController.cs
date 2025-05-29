using AutoMapper;
using BibliotecasAPI.Datos;
using BibliotecasAPI.DTO.AutorDTOs;
using BibliotecasAPI.Model.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    //[Route("api/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        
        [HttpGet] // api/autores
        [HttpGet("/lista-de-autores")] // /lista-de-autores
        public async Task<IEnumerable<AutorDTO>> Get()
        {
            var autores = await _context.Autores.Include(a => a.Libros).ToListAsync();
            var autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
            return autoresDTO;
        }

        
        [HttpGet("{id:int}", Name = "ObtenerAutor")] // api/autores/id
        public async Task<ActionResult<AutorConLibrosDTO>> Get(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.Libros)
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
            var autorDTO = _mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("ObtenerAutor", new { id = autorDTO.Id}, autorDTO);
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
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(int id)
        {
            //if (_context.Autores.Any(a => a.Id == id))
            //{
            //    _context.Remove(id);
            //    await _context.SaveChangesAsync();
            //    return Ok();
            //}
            //return NotFound();
            var registrosBorrados = await _context.Autores.Where(a => a.Id == id).ExecuteDeleteAsync();

            if (registrosBorrados == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
