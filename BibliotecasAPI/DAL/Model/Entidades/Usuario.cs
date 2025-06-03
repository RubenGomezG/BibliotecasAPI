using Microsoft.AspNetCore.Identity;

namespace BibliotecasAPI.DAL.Model.Entidades
{
    public class Usuario : IdentityUser
    {
        public DateTime FechaNacimiento { get; set; }
    }
}
