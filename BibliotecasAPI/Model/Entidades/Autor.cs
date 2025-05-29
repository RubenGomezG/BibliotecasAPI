using BibliotecasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.Model.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "El campo nombre es obligatorio")]
        [PrimeraLetraMayuscula]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Nombre { get; set; }


        [PrimeraLetraMayuscula]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Apellidos { get; set; }


        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Identificacion { get; set; }

        public List<Libro> Libros { get; set; } = new List<Libro>();

        //[Range(18,120)]
        //public int Edad { get; set; }

        //[CreditCard]
        //public string? TarjetaDeCredito { get; set; }

        //[Url]
        //public string? URL { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula - por modelo",
                        new string[] {nameof(Nombre)});
                }
            }
        }
    }
}
