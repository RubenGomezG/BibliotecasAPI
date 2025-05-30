using BibliotecasAPI.DAL.DTOs.AutorDTOs;

namespace BibliotecasAPI.DAL.DTOs.LibroDTOs
{
    public class LibroConAutoresDTO : LibroDTO
    {
        public List<AutorDTO> Autores { get; set; } = [];
    }
}
