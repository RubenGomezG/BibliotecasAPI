using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioAutores
    {
        public Task<IEnumerable<AutorDTO>> GetAutores(PaginacionDTO paginacionDTO);
        public Task<ActionResult<IEnumerable<AutorDTO>>> Filtrar(AutorFiltroDTO autorFiltroDTO);
        public Task<ActionResult<AutorConLibrosDTO>> GetAutorPorId(int id);
        public Task<ActionResult> AnadirAutor(AutorCreacionDTO autorCreacionDTO);
        public Task<ActionResult> AnadirAutorConFoto(AutorCreacionConFotoDTO autorCreacionDTO);
        public Task<ActionResult> ActualizarAutor(int id, AutorCreacionConFotoDTO autorCreacionDTO);
        public Task<ActionResult> PatchAutor(int id, JsonPatchDocument<AutorPatchDTO> patchDoc);
        public Task<ActionResult> BorrarAutor(int id);
    }
}
