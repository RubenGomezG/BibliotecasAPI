using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IGeneradorEnlaces
    {
        public Task GenerarEnlaces(AutorDTO autorDTO);
        public Task<ColeccionDeRecursosDTO<AutorDTO>> GenerarEnlaces(List<AutorDTO> autores);
    }
}
