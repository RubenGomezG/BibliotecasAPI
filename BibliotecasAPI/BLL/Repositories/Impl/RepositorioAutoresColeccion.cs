using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioAutoresColeccion : IRepositorioAutoresColeccion
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RepositorioAutoresColeccion(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //Este método sirve para obtener los ID de varios autores, separados como string por comas.
        public async Task<ActionResult<List<AutorConLibrosDTO>>> GetAutoresPorIds(string ids)
        {
            List<int> idsColeccion = new List<int>();
            foreach (var id in ids.Split(','))
            {
                if (int.TryParse(id, out int idInt))
                {
                    idsColeccion.Add(idInt);
                }
            }

            if (idsColeccion.Count == 0)
            {
                return new NotFoundResult();
            }

            List<Autor> autores = await _context.Autores
                            .Include(a => a.Libros)
                            .ThenInclude(al => al.Libro)
                            .Where(l => idsColeccion.Contains(l.Id)).ToListAsync();

            if (autores.Count != idsColeccion.Count)
            {
                return new NotFoundResult();
            }
            List<AutorConLibrosDTO> autoresDTO = _mapper.Map<List<AutorConLibrosDTO>>(autores);
            return autoresDTO;
        }

        public async Task<ActionResult> AnadirVariosAutores(IEnumerable<AutorCreacionDTO> autoresCreacionDTO, string nombreEndpoint)
        {
            IEnumerable<Autor> autores = _mapper.Map<IEnumerable<Autor>>(autoresCreacionDTO);
            _context.AddRange(autores);
            await _context.SaveChangesAsync();

            IEnumerable<AutorDTO> autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
            IEnumerable<int> ids = autores.Select(a => a.Id);
            string idsString = string.Join(",", ids);
            return new CreatedAtRouteResult(nombreEndpoint, new { ids = idsString }, autoresDTO);
        }
    }
}
