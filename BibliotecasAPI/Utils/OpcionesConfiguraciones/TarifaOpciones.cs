using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.Utils.OpcionesConfiguraciones
{
    public class TarifaOpciones
    {
        public const string Seccion = "Tarifas";
        [Required]
        public required double Dia { get; set; }
        [Required]
        public required double Noche { get; set; }
    }
}
