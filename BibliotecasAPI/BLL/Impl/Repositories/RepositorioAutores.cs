using AutoMapper;
using BibliotecasAPI.BLL.Interfaces.IRepositories;
using BibliotecasAPI.BLL.Interfaces.IServices;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BibliotecasAPI.BLL.Impl.Repositories
{
    public class RepositorioAutores : IRepositorioAutores
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly IMapper _mapper;
        private readonly ILogger<RepositorioAutores> _logger;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string CONTENEDOR = "autores";
        private const string CACHE_AUTORES = "autores-obtener";

        public RepositorioAutores(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IOutputCacheStore outputCacheStore,
            ILogger<RepositorioAutores> logger,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _outputCacheStore = outputCacheStore;
            _logger = logger;
            _almacenadorArchivos = almacenadorArchivos;
        }

        public async Task<IEnumerable<AutorDTO>> GetAutores(PaginacionDTO paginacionDTO)
        {
            var queryable = _context.Autores.Include(a => a.Libros).AsQueryable();
            await _httpContextAccessor.HttpContext!.InsertarParametrosPaginacionEnCabecera(queryable);

            var autores = await queryable
                            .OrderBy(a => a.Nombre)
                            .Paginar(paginacionDTO)
                            .ToListAsync();

            var autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
            return autoresDTO;
        }

        public async Task<ActionResult<IEnumerable<AutorDTO>>> Filtrar(AutorFiltroDTO autorFiltroDTO)
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

            await _httpContextAccessor.HttpContext!.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable
                        .Paginar(autorFiltroDTO.PaginacionDTO)
                        .ToListAsync();

            if (autorFiltroDTO.IncluirLibros)
            {
                var autoresDTO = _mapper.Map<IEnumerable<AutorConLibrosDTO>>(autores);
                return new OkObjectResult(autoresDTO);
            }
            else
            {
                var autoresDTO = _mapper.Map<IEnumerable<AutorDTO>>(autores);
                return new OkObjectResult(autoresDTO);
            }
        }

        public async Task<ActionResult<AutorConLibrosDTO>> GetAutorPorId(int id)
        {
            var autor = await _context.Autores
                .Include(a => a.Libros)
                .ThenInclude(l => l.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return new NotFoundResult();
            }

            var autorDTO = _mapper.Map<AutorConLibrosDTO>(autor);
            return new OkObjectResult(autorDTO);
        }

        public async Task<ActionResult<AutorConLibrosDTO>> GetAutorPorIdV2(int id, bool incluirLibros = false)
        {
            var queryable = _context.Autores.AsQueryable();

            if (incluirLibros)
            {
                queryable = queryable.Include(a => a.Libros)
                .ThenInclude(l => l.Libro);
            }

            var autor = await queryable.FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return new NotFoundResult();
            }

            var autorDTO = _mapper.Map<AutorConLibrosDTO>(autor);
            return new OkObjectResult(autorDTO);
        }

        public async Task<ActionResult> AnadirAutor(AutorCreacionDTO autorCreacionDTO)
        {
            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            _context.Add(autor);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);

            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return new CreatedAtRouteResult("ObtenerAutorV1", new { id = autorDTO.Id }, autorDTO);
        }

        public async Task<ActionResult> AnadirAutorConFoto(AutorCreacionConFotoDTO autorCreacionDTO)
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
            return new CreatedAtRouteResult("ObtenerAutorV1", new { id = autorDTO.Id }, autorDTO);
        }

        public async Task<ActionResult> ActualizarAutor(int id, AutorCreacionConFotoDTO autorCreacionDTO)
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
                return new NoContentResult();
            }
            return new NotFoundResult();
        }

        public async Task<ActionResult> PatchAutor(Autor autorDB, AutorPatchDTO autorPatchDTO)
        {
            _mapper.Map(autorPatchDTO, autorDB);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);
            return new NoContentResult();
        }

        public async Task<ActionResult> BorrarAutor(int id)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return new NotFoundResult();
            }

            _context.Remove(autor);
            await _context.SaveChangesAsync();

            await _outputCacheStore.EvictByTagAsync(CACHE_AUTORES, default);
            await _almacenadorArchivos.Borrar(autor.Foto, CONTENEDOR);
            return new NoContentResult();
        }
    }
}
