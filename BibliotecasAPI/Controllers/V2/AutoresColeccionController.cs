using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V2
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v2/autores-coleccion")]
    public class AutoresColeccionController : ControllerBase
    {
        private readonly IServicioAutoresColeccion _servicioAutoresColeccion;

        public AutoresColeccionController(IServicioAutoresColeccion servicioAutoresColeccion)
        {
            _servicioAutoresColeccion = servicioAutoresColeccion;
        }

        [HttpGet("{ids}", Name= "ObtenerAutoresPorIdsV2")]
        public async Task<ActionResult<List<AutorConLibrosDTO>>> Get(string ids)
        {
            return await _servicioAutoresColeccion.ObtenerAutoresPorIds(ids);
        }

        [HttpPost(Name = "CrearAutoresV2")]
        public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autoresCreacionDTO)
        {
            return await _servicioAutoresColeccion.AnadirVariosAutores(autoresCreacionDTO, "ObtenerAutoresPorIdsV2");
        }
    }
}
