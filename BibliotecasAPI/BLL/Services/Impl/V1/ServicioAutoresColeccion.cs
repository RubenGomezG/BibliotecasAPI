using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioAutoresColeccion : IServicioAutoresColeccion
    {
        private readonly IRepositorioAutoresColeccion _repositorioAutoresColeccion;

        public ServicioAutoresColeccion(IRepositorioAutoresColeccion repositorioAutoresColeccion)
        {
            _repositorioAutoresColeccion = repositorioAutoresColeccion;
        }
        public async Task<ActionResult> AnadirVariosAutores(IEnumerable<AutorCreacionDTO> autoresCreacionDTO, string nombreEndpoint)
        {
            return await _repositorioAutoresColeccion.AnadirVariosAutores(autoresCreacionDTO, nombreEndpoint);
        }

        public async Task<ActionResult<List<AutorConLibrosDTO>>> ObtenerAutoresPorIds(string ids)
        {
            return await _repositorioAutoresColeccion.GetAutoresPorIds(ids);
        }
    }
}
