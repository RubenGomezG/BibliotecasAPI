using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.ComentarioDTOs
{
    public class ComentarioDTO
    {
        public Guid Id { get; set; }
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
