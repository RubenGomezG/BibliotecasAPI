using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.PeticionDTOs
{
    public class LimitarPeticionesDTO
    {
        public const string Seccion = "limitarPeticiones";
        [Required]
        public int PeticionesPorDiaGratuito { get; set; }
    }
}
