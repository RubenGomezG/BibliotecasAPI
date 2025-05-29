using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DTO.LibroDTOs
{
    public class LibroCreacionDTO
    {
        [Required]
        [StringLength(200, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Titulo { get; set; }
        public int AutorId { get; set; }
    }
}
