using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioLibros : IServicioLibros
    {
        private readonly IRepositorioLibros _repositorioLibros;

        public ServicioLibros(IRepositorioLibros repositorioLibros)
        {
            _repositorioLibros = repositorioLibros;
        }

        public async Task<IEnumerable<LibroConAutoresDTO>> GetLibros(PaginacionDTO paginacionDTO)
        {
            return await _repositorioLibros.GetLibros(paginacionDTO);
        }
        public async Task<ActionResult<LibroConAutoresDTO>> GetLibroPorId(int id) 
        {
            return await _repositorioLibros.GetLibroPorId(id);
        }
        public async Task<ActionResult> AnadirLibro(LibroCreacionDTO libroCreacionDTO) 
        {
            return await _repositorioLibros.AnadirLibro(libroCreacionDTO);
        }
        public async Task<ActionResult> ActualizarLibro(int id, LibroCreacionDTO libroCreacionDTO) 
        {
            return await _repositorioLibros.ActualizarLibro(id, libroCreacionDTO);
        }
        public async Task<ActionResult> BorrarLibro(int id)
        {
            return await _repositorioLibros.BorrarLibro(id);
        }
    }
}
