using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public RepositorioUsuarios(UserManager<Usuario> userManager, IHttpContextAccessor contextAccessor)
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
