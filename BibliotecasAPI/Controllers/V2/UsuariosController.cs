using AutoMapper;
using BibliotecasAPI.BLL.Interfaces.IServices;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
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
            var usuario = new Usuario
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO?.Email
            };

            var resultado = await _userManager.CreateAsync(usuario, credencialesUsuarioDTO!.Password!);
            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO);
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
            var usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario is null)
            {
                return RetornarLoginIncorrecto();   
            }

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO!.Password!, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                return RetornarLoginIncorrecto();
            }
        }

        [HttpGet]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            var usuarios = await _context.Users.ToListAsync();
            var usuariosDTO = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
            return Ok(usuariosDTO);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Put(ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            var usuario = await _servicioUsuarios.ObtenerUsuario();

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
            var usuario = await _servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }
            var credencialesUsuarioDTo = new CredencialesUsuarioDTO { Email = usuario.Email! };

            var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTo);
            return respuestaAutenticacion;
        }

        [HttpPost("hacer-admin")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(editarClaimDTO.Email);

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
            var usuario = await _userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "true"));
            return NoContent();
        }

        private ActionResult RetornarLoginIncorrecto()
        {
            ModelState.AddModelError(string.Empty, "Login Incorrecto");
            return ValidationProblem();
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var claims = new List<Claim> 
            {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("almudena", "rosi")
            };

            var usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await _userManager.GetClaimsAsync(usuario!);
            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["LlaveJWT"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);
            return new RespuestaAutenticacionDTO
            {
                Token = token,
                FechaExpiracion = expiracion,
            };
        }
    }
}
