using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecasAPI.Controllers.V2
{
    [ApiController]
    [Route("api/v2/usuarios")]
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

        [HttpPost("registro")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(
            CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            Usuario usuario = new Usuario
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO?.Email
            };

            IdentityResult resultado = await _userManager.CreateAsync(usuario, credencialesUsuarioDTO!.Password!);
            if (resultado.Succeeded)
            {
                RespuestaAutenticacionDTO respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return ValidationProblem();
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(
            CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            Usuario? usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario is null)
            {
                return ModelState.RetornarLoginIncorrecto();   
            }

            Microsoft.AspNetCore.Identity.SignInResult resultado = await _signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO!.Password!, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                RespuestaAutenticacionDTO respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                return ModelState.RetornarLoginIncorrecto();
            }
        }

        [HttpGet]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            List<Usuario> usuarios = await _context.Users.ToListAsync();
            IEnumerable<UsuarioDTO> usuariosDTO = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
            return Ok(usuariosDTO);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Put(ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            Usuario? usuario = await _servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }

            usuario.FechaNacimiento = actualizarUsuarioDTO.FechaNacimiento;

            await _userManager.UpdateAsync(usuario);
            return NoContent();
        }

        [HttpGet("renovar-token")]
        [Authorize]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            Usuario? usuario = await _servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }
            CredencialesUsuarioDTO credencialesUsuarioDTo = new CredencialesUsuarioDTO { Email = usuario.Email! };

            RespuestaAutenticacionDTO respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTo);
            return respuestaAutenticacion;
        }

        [HttpPost("hacer-admin")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
        {
            Usuario? usuario = await _userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario == null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(usuario, new Claim ("esAdmin", "true"));
            return NoContent();
        }

        [HttpPost("quitar-admin")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> QuitarAdmin(EditarClaimDTO editarClaimDTO)
        {
            Usuario? usuario = await _userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "true"));
            return NoContent();
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("almudena", "rosi")
            };

            Usuario? usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            IList<Claim> claimsDB = await _userManager.GetClaimsAsync(usuario!);
            claims.AddRange(claimsDB);

            SymmetricSecurityKey llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["LlaveJWT"]!));
            SigningCredentials credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            DateTime expiracion = DateTime.UtcNow.AddYears(1);

            JwtSecurityToken tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: credenciales);

            string token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);
            return new RespuestaAutenticacionDTO
            {
                Token = token,
                FechaExpiracion = expiracion,
            };
        }
    }
}
