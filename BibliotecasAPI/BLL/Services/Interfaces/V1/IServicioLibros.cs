using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioLibros
    {
        public Task<IEnumerable<LibroConAutoresDTO>> GetLibros(PaginacionDTO paginacionDTO);
        public Task<ActionResult<LibroConAutoresDTO>> GetLibroPorId(int id);
        public Task<ActionResult> AnadirLibro(LibroCreacionDTO libroCreacionDTO, string nombreEndpoint);
        public Task<ActionResult> ActualizarLibro(int id, LibroCreacionDTO libroCreacionDTO);
        public Task<ActionResult> BorrarLibro(int id);
    }
}
