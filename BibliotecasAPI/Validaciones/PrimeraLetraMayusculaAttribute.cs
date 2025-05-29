using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            //Como ya existe una validación por defecto que comprueba esto, el tag [Required], nosotros aceptamos el valor y delegamos en [Required] realizar esta tarea.
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()![0].ToString();
            return primeraLetra != primeraLetra.ToUpper() ? new ValidationResult("La primera letra debe ser mayúscula") : ValidationResult.Success;
        }
    }
}
