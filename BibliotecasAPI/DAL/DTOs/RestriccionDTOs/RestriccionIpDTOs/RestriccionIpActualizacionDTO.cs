using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs
{
    public class RestriccionIpActualizacionDTO
    {
        [Required]
        public required string Ip { get; set; }
    }
}
