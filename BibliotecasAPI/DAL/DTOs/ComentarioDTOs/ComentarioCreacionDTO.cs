using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.ComentarioDTOs
{
    public class ComentarioCreacionDTO
    {
        [Required]
        public required string Cuerpo { get; set; }
    }
}
