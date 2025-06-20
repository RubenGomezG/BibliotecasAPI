using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly IRepositorioUsuarios _repositorioUsuarios;

        public ServicioUsuarios(IRepositorioUsuarios repositorioUsuarios)
        {
            _repositorioUsuarios = repositorioUsuarios;
        }

        public async Task<Usuario?> ObtenerUsuario()
        {
            return await _repositorioUsuarios.ObtenerUsuario();
        }

        public string? ObtenerUsuarioId()
        {
            return _repositorioUsuarios.ObtenerUsuarioId();
        }
    }
}
