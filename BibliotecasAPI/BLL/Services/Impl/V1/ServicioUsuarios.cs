using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ServicioUsuarios(UserManager<Usuario> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
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
