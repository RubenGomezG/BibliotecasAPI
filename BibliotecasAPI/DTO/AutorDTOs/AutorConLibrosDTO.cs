using BibliotecasAPI.DTO.LibroDTOs;

namespace BibliotecasAPI.DTO.AutorDTOs
{
    public class AutorConLibrosDTO : AutorDTO
    {
        public List<LibroDTO> Libros { get; set; } = [];
    }
}
