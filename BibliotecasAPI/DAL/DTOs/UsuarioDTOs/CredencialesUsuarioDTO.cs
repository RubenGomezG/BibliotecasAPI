using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.UsuarioDTOs
{
    public class CredencialesUsuarioDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
