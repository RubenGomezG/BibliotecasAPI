using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            return await _repositorioUsuarios.Registrar(credencialesUsuarioDTO);
        }

        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            return await _repositorioUsuarios.Login(credencialesUsuarioDTO);
        }

        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ObtenerUsuarios()
        {
            return await _repositorioUsuarios.ObtenerUsuarios();
        }

        public async Task<ActionResult> ActualizarUsuario(ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            return await _repositorioUsuarios.ActualizarUsuario(actualizarUsuarioDTO);
        }

        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            return await _repositorioUsuarios.RenovarToken();
        }

        public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
        {
            return await _repositorioUsuarios.HacerAdmin(editarClaimDTO);
        }

        public async Task<ActionResult> QuitarAdmin(EditarClaimDTO editarClaimDTO)
        {
            return await _repositorioUsuarios.HacerAdmin(editarClaimDTO);
        }
    }
}
