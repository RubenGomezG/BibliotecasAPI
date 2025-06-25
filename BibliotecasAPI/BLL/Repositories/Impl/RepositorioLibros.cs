using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioLibros : IRepositorioLibros
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CACHE_LIBROS = "libros-obtener";

        public RepositorioLibros(ApplicationDbContext context, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<IEnumerable<LibroConAutoresDTO>> GetLibros(PaginacionDTO paginacionDTO)
        {
            var queryable = _context.Libros
                            .Include(l => l.Autores)
                            .ThenInclude(a => a.Autor)
                            .AsQueryable();

            await _httpContextAccessor.HttpContext!.InsertarParametrosPaginacionEnCabecera(queryable);
            var libros = await queryable
                            .OrderBy(a => a.Titulo)
                            .Paginar(paginacionDTO)
                            .ToListAsync();

            var librosDTO = _mapper.Map<IEnumerable<LibroConAutoresDTO>>(libros);
            return librosDTO;
        }

        public async Task<ActionResult<LibroConAutoresDTO>> GetLibroPorId(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.Autores)
                .ThenInclude(a => a.Autor)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro is null)
            {
                return new NotFoundResult();
            }

            var libroDTO = _mapper.Map<LibroConAutoresDTO>(libro);
            return libroDTO;
        }

        public async Task<ActionResult> AnadirLibro(LibroCreacionDTO libroCreacionDTO)
        {
            var libro = _mapper.Map<Libro>(libroCreacionDTO);
            LibroUtils.AsignarOrdenAutores(libro);

            _context.Add(libro);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);

            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return new CreatedAtRouteResult("ObtenerLibroV1", new { id = libro.Id }, libroDTO);
        }

        public async Task<ActionResult> ActualizarLibro(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await _context.Libros
                            .Include(l => l.Autores)
                            .FirstOrDefaultAsync(l => l.Id == id);

            if (libroDB == null)
            {
                return new NotFoundResult();
            }

            libroDB = _mapper.Map(libroCreacionDTO, libroDB);
            LibroUtils.AsignarOrdenAutores(libroDB);

            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);
            return new NoContentResult();
        }

        public async Task<ActionResult> BorrarLibro(int id)
        {
            var registrosBorrados = await _context.Libros.Where(a => a.Id == id).ExecuteDeleteAsync();

            if (registrosBorrados == 0)
            {
                return new NotFoundResult();
            }

            await _outputCacheStore.EvictByTagAsync(CACHE_LIBROS, default);
            return new NoContentResult();
        }
    }
}
