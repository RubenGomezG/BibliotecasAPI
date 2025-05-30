using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.Utils.OpcionesConfiguraciones
{
    public class PersonaOpciones
    {
        public const string Seccion = "Seccion_1";
        [Required]
        public required string Nombre { get; set; }
        [Required]
        public int Edad { get; set; }
    }
}
