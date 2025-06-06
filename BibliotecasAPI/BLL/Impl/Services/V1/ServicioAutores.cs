using BibliotecasAPI.BLL.Interfaces.IRepositories;
using BibliotecasAPI.BLL.Interfaces.IServices.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Impl.Services.V1
{
    public class ServicioAutores : IServicioAutores
    {
        private readonly IRepositorioAutores _repositorioAutores;

        public ServicioAutores(IRepositorioAutores repositorioAutores)
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

        public async Task<ActionResult<AutorConLibrosDTO>> GetAutorPorId(int id)
        {
            return await _repositorioAutores.GetAutorPorId(id);
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

        public async Task<ActionResult> PatchAutor(Autor autorDB, AutorPatchDTO autorPatchDTO)
        {
            return await _repositorioAutores.PatchAutor(autorDB, autorPatchDTO);
        }

        public async Task<ActionResult> BorrarAutor(int id)
        {
            return await _repositorioAutores.BorrarAutor(id);
        }
    }
}
