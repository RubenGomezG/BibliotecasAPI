using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
        string? ObtenerUsuarioId();
        public Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO);
        public Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO);
        public Task<ActionResult<IEnumerable<UsuarioDTO>>> ObtenerUsuarios();
        public Task<ActionResult> ActualizarUsuario(ActualizarUsuarioDTO actualizarUsuarioDTO);
        public Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken();
        public Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO);
        public Task<ActionResult> QuitarAdmin(EditarClaimDTO editarClaimDTO);
    }
}
