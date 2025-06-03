using BibliotecasAPI.Utils.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.Model.Entidades
{
    public class Autor
    {
        public int Id { get; set; }

        [PrimeraLetraMayuscula]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Nombre { get; set; }

        [PrimeraLetraMayuscula]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Apellidos { get; set; }

        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public string? Identificacion { get; set; }
        [Unicode(false)]
        public string? Foto { get; set; }
        public List<AutorLibro> Libros { get; set; } = [];
    }
}
