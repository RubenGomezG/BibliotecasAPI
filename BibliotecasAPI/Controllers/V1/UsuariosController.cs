using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.Utils.Attributes;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [DeshabilitarLimitePeticiones]
    public class UsuariosController : ControllerBase
    {
        private readonly IServicioUsuarios _servicioUsuarios;

        public UsuariosController(IServicioUsuarios servicioUsuarios)
        {
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpPost("registro", Name = "RegistrarV1")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(
            CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            return await _servicioUsuarios.Registrar(credencialesUsuarioDTO);
        }

        [HttpPost("login", Name = "LoginV1")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            ActionResult<RespuestaAutenticacionDTO> result = await _servicioUsuarios.Login(credencialesUsuarioDTO);
            StatusCodeResult? codigo = result.Result as StatusCodeResult;
            if (codigo!.StatusCode == 404)
            {
                ModelState.RetornarLoginIncorrecto();
            }
            return result;
        }

        [HttpGet(Name = "ObtenerUsuariosV1")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            return await _servicioUsuarios.ObtenerUsuarios();
        }

        [HttpPut(Name = "ActualizarUsuarioV1")]
        [Authorize]
        public async Task<ActionResult> Put(ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            return await _servicioUsuarios.ActualizarUsuario(actualizarUsuarioDTO);
        }

        [HttpGet("renovar-token", Name = "RenovarTokenV1")]
        [Authorize]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            return await _servicioUsuarios.RenovarToken();
        }

        [HttpPost("hacer-admin", Name = "HacerAdminV1")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
        {
            return await _servicioUsuarios.HacerAdmin(editarClaimDTO);
        }

        [HttpPost("quitar-admin", Name = "QuitarAdminV1")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> QuitarAdmin(EditarClaimDTO editarClaimDTO)
        {
            return await _servicioUsuarios.QuitarAdmin(editarClaimDTO);
        }
    }
}