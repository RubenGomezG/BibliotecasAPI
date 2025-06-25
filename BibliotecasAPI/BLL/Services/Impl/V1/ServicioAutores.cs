using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
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
            return await _repositorioAutores.AnadirAutorConFoto(autorCreacionDTO);
        }

        public async Task<ActionResult> ActualizarAutor(int id, AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _repositorioAutores.ActualizarAutor(id, autorCreacionDTO);
        }
        public async Task<ActionResult> PatchAutor(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
        {
            return await _repositorioAutores.PatchAutor(id, patchDoc);
        }

        public async Task<ActionResult> BorrarAutor(int id)
        {
            return await _repositorioAutores.BorrarAutor(id);
        }
    }
}
