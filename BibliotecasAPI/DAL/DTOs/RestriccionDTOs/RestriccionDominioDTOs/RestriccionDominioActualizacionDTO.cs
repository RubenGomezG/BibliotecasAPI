using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs
{
    public class RestriccionDominioActualizacionDTO
    {
        [Required]
        public required string Dominio { get; set; }
    }
}
