using BibliotecasAPI.DAL.DTOs.LibroDTOs;

namespace BibliotecasAPI.DAL.DTOs.AutorDTOs
{
    public class AutorConLibrosDTO : AutorDTO
    {
        public List<LibroDTO> Libros { get; set; } = [];
    }
}
