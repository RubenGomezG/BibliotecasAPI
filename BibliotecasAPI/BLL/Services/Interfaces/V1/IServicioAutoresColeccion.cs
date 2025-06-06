using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioAutoresColeccion
    {
        public Task<ActionResult> AnadirVariosAutores(IEnumerable<AutorCreacionDTO> autoresCreacionDTO);
        public Task<ActionResult<List<AutorConLibrosDTO>>> ObtenerAutoresPorIds(string ids);
    }
}
