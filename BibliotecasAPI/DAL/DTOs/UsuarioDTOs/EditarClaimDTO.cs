using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.UsuarioDTOs
{
    public class EditarClaimDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
