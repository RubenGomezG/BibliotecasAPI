using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BibliotecasAPI.Controllers.V2
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v2/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly IServicioLibros _servicioLibros;
        private const string CACHE_LIBROS = "libros-obtener";

        public LibrosController(IServicioLibros servicioLibros)
        {
            _servicioLibros = servicioLibros;
        }

        [HttpGet(Name = "ObtenerLibrosV2")]
        [AllowAnonymous]
        [OutputCache(Tags = [CACHE_LIBROS])]
        public async Task<IEnumerable<LibroConAutoresDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await _servicioLibros.GetLibros(paginacionDTO);
        }
          
        [HttpGet("{id:int}", Name = "ObtenerLibroV2")] //api/libros/{id}
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
        {
            return await _servicioLibros.GetLibroPorId(id);
        }

        [HttpPost(Name = "CrearLibroV2")]
        [ServiceFilter<FiltroValidacionLibro>()]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            return await _servicioLibros.AnadirLibro(libroCreacionDTO, "ObtenerLibroV2");
        }

        [HttpPut("{id:int}", Name = "ActualizarLibroV2")] //Put/api/libros/{id}
        [ServiceFilter<FiltroValidacionLibro>()]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            return await _servicioLibros.ActualizarLibro(id, libroCreacionDTO);
        }

        [HttpDelete("{id:int}", Name = "BorrarLibroV2")] //Put/api/libros/{id}
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioLibros.BorrarLibro(id);
        }
    }
}
