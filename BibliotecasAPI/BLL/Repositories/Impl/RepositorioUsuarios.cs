using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.ClassUtils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RepositorioUsuarios(UserManager<Usuario> userManager,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration,
            SignInManager<Usuario> signInManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            Usuario usuario = new Usuario
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO?.Email
            };

            IdentityResult resultado = await _userManager.CreateAsync(usuario, credencialesUsuarioDTO!.Password!);
            if (resultado.Succeeded)
            {
                RespuestaAutenticacionDTO respuestaAutenticacion = await UserUtils.ConstruirToken(credencialesUsuarioDTO, _configuration, _userManager, usuario.Id);
                LlaveAPI llaveAPI = LlaveApiUtils.CrearLlave(_context, usuario.Id, TipoLlave.Gratuita);
                _context.Add(llaveAPI);
                await _context.SaveChangesAsync();
                return respuestaAutenticacion;
            }
            else
            {
                throw new ValidationException();
            }
        }

        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            Usuario? usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (!UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }

            Microsoft.AspNetCore.Identity.SignInResult resultado = await _signInManager.CheckPasswordSignInAsync(usuario!, credencialesUsuarioDTO!.Password!, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                RespuestaAutenticacionDTO respuestaAutenticacion = await UserUtils.ConstruirToken(credencialesUsuarioDTO, _configuration, _userManager, usuario.Id);
                return respuestaAutenticacion;
            }
            else
            {
                return new NotFoundResult();
            }
        }

        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ObtenerUsuarios()
        {
            var usuarios = await _context.Users.ToListAsync();
            var usuariosDTO = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
            return new OkObjectResult(usuariosDTO);
        }

        public async Task<ActionResult> ActualizarUsuario(ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            var usuario = await ObtenerUsuario();

            if (!UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }

            usuario!.FechaNacimiento = actualizarUsuarioDTO.FechaNacimiento;

            await _userManager.UpdateAsync(usuario);
            return new NoContentResult();
        }

        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            var usuario = await ObtenerUsuario();
            if (!UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }
            var credencialesUsuarioDTO = new CredencialesUsuarioDTO { Email = usuario!.Email! };

            var respuestaAutenticacion = await UserUtils.ConstruirToken(credencialesUsuarioDTO, _configuration, _userManager, usuario.Id);
            return respuestaAutenticacion;
        }

        public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }

            await _userManager.AddClaimAsync(usuario!, new Claim("esAdmin", "true"));
            return new NoContentResult();
        }

        public async Task<ActionResult> QuitarAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (!UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }

            await _userManager.RemoveClaimAsync(usuario!, new Claim("esAdmin", "true"));
            return new NoContentResult();
        }

        public string? ObtenerUsuarioId()
        {
            Claim? idClaim = _contextAccessor.HttpContext!
                .User.Claims.Where(claim => claim.Type == "usuarioId").FirstOrDefault();

            if (idClaim == null)
            {
                return null;
            }
            string id = idClaim.Value;
            return id;
        }

        public async Task<Usuario?> ObtenerUsuario()
        {
            Claim? emailClaim = _contextAccessor.HttpContext!
                .User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            if (emailClaim == null)
            {
                return null;
            }

            string email = emailClaim.Value;
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
