using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V2;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;


namespace BibliotecasAPI.BLL.Services.Impl.V2
{
    public class ServicioAutoresV2 : IServicioAutoresV2
    {
        private readonly IRepositorioAutores _repositorioAutores;

        public ServicioAutoresV2(IRepositorioAutores repositorioAutores)
        {
            _repositorioAutores = repositorioAutores;
        }

        public async Task<IEnumerable<AutorDTO>> GetAutores(PaginacionDTO paginacionDTO)
        {
            return await _repositorioAutores.GetAutores(paginacionDTO);
        }

        public async Task<ActionResult<IEnumerable<AutorDTO>>> Filtrar(AutorFiltroDTO autorFiltroDTO)
        {
            return await _repositorioAutores.Filtrar(autorFiltroDTO);
        }

        public async Task<ActionResult<AutorConLibrosDTO>> GetAutorPorIdV2(int id, bool incluirLibros = false)
        {
            return await _repositorioAutores.GetAutorPorIdV2(id, incluirLibros);
        }

        public async Task<ActionResult> AnadirAutor(AutorCreacionDTO autorCreacionDTO)
        {
            return await _repositorioAutores.AnadirAutor(autorCreacionDTO);
        }

        public async Task<ActionResult> AnadirAutorConFoto(AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await AnadirAutorConFoto(autorCreacionDTO);
        }

        public async Task<ActionResult> ActualizarAutor(int id, AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _repositorioAutores.ActualizarAutor(id, autorCreacionDTO);
        }

        public async Task<ActionResult> BorrarAutor(int id)
        {
            return await _repositorioAutores.BorrarAutor(id);
        }
    }
}
