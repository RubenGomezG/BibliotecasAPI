using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioAutores
    {
        public Task<IEnumerable<AutorDTO>> GetAutores(PaginacionDTO paginacionDTO);
        public Task<ActionResult<IEnumerable<AutorDTO>>> Filtrar(AutorFiltroDTO autorFiltroDTO);
        public Task<AutorConLibrosDTO> GetAutorPorId(int id);
        public Task<ActionResult<AutorConLibrosDTO>> GetAutorPorIdV2(int id, bool incluirLibros = false);
        public Task<ActionResult> AnadirAutor(AutorCreacionDTO autorCreacionDTO);
        public Task<ActionResult> AnadirAutorConFoto(AutorCreacionConFotoDTO autorCreacionDTO);
        public Task<ActionResult> ActualizarAutor(int id, AutorCreacionConFotoDTO autorCreacionDTO);
        public Task<ActionResult> BorrarAutor(int id);
    }
}
