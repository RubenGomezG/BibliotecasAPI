using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs
{
    public class RestriccionIpCreacionDTO
    {
        public int LlaveId { get; set; }
        [Required]
        public required string Ip { get; set; }
    }
}
