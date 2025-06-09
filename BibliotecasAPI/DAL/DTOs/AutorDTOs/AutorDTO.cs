using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.AutorDTOs
{
    public class AutorDTO : RecursoDTO
    {
        public int Id { get; set; }
        public required string NombreCompleto { get; set; }
        public string? Foto { get; set; }
    }
}
