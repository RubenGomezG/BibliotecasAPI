using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Interfaces.IRepositories
{
    public interface IRepositorioAutoresColeccion
    {
        public Task<ActionResult> AnadirVariosAutores(IEnumerable<AutorCreacionDTO> autoresCreacionDTO);
        public Task<ActionResult<List<AutorConLibrosDTO>>> GetAutoresPorIds(string ids);
    }
}
