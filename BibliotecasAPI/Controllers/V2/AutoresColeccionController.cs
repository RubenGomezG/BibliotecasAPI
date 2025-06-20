﻿using AutoMapper;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V2
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v2/autores-coleccion")]
    public class AutoresColeccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresColeccionController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{ids}", Name= "ObtenerAutoresPorIdsV2")]
        public async Task<ActionResult<List<AutorConLibrosDTO>>> Get(string ids)
        {
            var idsColeccion = new List<int>();
            foreach (var id in ids.Split(','))
            {
                if (int.TryParse(id, out int idInt))
                {
                    idsColeccion.Add(idInt);
                }
            }

            if (idsColeccion.Count == 0)
            {
                ModelState.AddModelError(nameof(ids), "Ningún ID fue encontrado.");
                return ValidationProblem();
            }

            var autores = await _context.Autores
                            .Include(a => a.Libros)
                            .ThenInclude(al => al.Libro)
                            .Where(l => idsColeccion.Contains(l.Id)).ToListAsync();

            if (autores.Count != idsColeccion.Count)
            {
                return NotFound();
            }
            var autoresDTO = _mapper.Map<List<AutorConLibrosDTO>>(autores);
            return autoresDTO;
        }

        [HttpPost(Name = "CrearAutoresV2")]
        public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autoresCreacionDTO)
        {
            var autores = _mapper.Map<IEnumerable<Autor>>(autoresCreacionDTO);
            _context.AddRange(autores);
            await _context.SaveChangesAsync();

            var autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
            var ids = autores.Select(a => a.Id);
            var idsString = string.Join(",", ids);
            return CreatedAtRoute("ObtenerAutoresPorIdsV2", new { ids = idsString }, autoresDTO);
        }
    }
}
