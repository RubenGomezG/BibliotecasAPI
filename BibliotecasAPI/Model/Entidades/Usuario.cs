using Microsoft.AspNetCore.Identity;

namespace BibliotecasAPI.Model.Entidades
{
    public class Usuario : IdentityUser
    {
        public DateTime FechaNacimiento { get; set; }
    }
}
