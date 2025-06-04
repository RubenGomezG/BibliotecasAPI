using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Validation;
using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.AutorDTOs
{
    public class AutorCreacionDTO
    {
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

        public List<LibroCreacionDTO> Libros { get; set; } = [];
    }
}
