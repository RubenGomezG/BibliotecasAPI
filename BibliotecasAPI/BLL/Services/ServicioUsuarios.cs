using BibliotecasAPI.BLL.IServices;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BibliotecasAPI.BLL.Services
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ServicioUsuarios(UserManager<Usuario> userManager, IHttpContextAccessor contextAccessor)
        {
            this._userManager = userManager;
            this._contextAccessor = contextAccessor;
        }

        public async Task<Usuario?> ObtenerUsuario()
        {
            var emailClaim = _contextAccessor.HttpContext!
                .User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            if (emailClaim == null)
            {
                return null;
            }

            var email = emailClaim.Value;
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
