using BibliotecasAPI.BLL.Interfaces.IRepositories;
using BibliotecasAPI.BLL.Interfaces.IServices.V1;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Impl.Services.V1
{
    public class ServicioAutoresColeccion : IServicioAutoresColeccion
    {
        private readonly IRepositorioAutoresColeccion _repositorioAutoresColeccion;

        public ServicioAutoresColeccion(IRepositorioAutoresColeccion repositorioAutoresColeccion)
        {
            _repositorioAutoresColeccion = repositorioAutoresColeccion;
        }
        public async Task<ActionResult> AnadirVariosAutores(IEnumerable<AutorCreacionDTO> autoresCreacionDTO)
        {
            return await _repositorioAutoresColeccion.AnadirVariosAutores(autoresCreacionDTO);
        }

        public async Task<ActionResult<List<AutorConLibrosDTO>>> ObtenerAutoresPorIds(string ids)
        {
            return await _repositorioAutoresColeccion.GetAutoresPorIds(ids);
        }
    }
}
