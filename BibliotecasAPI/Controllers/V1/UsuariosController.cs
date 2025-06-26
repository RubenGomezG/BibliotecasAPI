using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Attributes;
using BibliotecasAPI.Utils.ClassUtils;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BibliotecasAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [DeshabilitarLimitePeticiones]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsuariosController(UserManager<Usuario> userManager,
            IConfiguration configuration,
            SignInManager<Usuario> signInManager,
            IServicioUsuarios servicioUsuarios,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _servicioUsuarios = servicioUsuarios;
            _context = context;
            _mapper = mapper;
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
