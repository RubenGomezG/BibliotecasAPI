using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs
{
    public class RestriccionIpDTO
    {
        public int Id { get; set; }
        [Required]
        public required string Ip { get; set; }
    }
}
