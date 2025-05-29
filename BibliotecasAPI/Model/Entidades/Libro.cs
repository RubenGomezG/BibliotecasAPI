using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.Model.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor? Autor { get; set; }
    }
}
