﻿using AutoMapper;
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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly IServicioLlaves _servicioLlaves;

        public UsuariosController(UserManager<Usuario> userManager,
            IConfiguration configuration,
            SignInManager<Usuario> signInManager,
            IServicioUsuarios servicioUsuarios,
            ApplicationDbContext context,
            IMapper mapper,
            IServicioLlaves servicioLlaves)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _servicioUsuarios = servicioUsuarios;
            _context = context;
            _mapper = mapper;
            _servicioLlaves = servicioLlaves;
        }

        [HttpPost("registro", Name = "RegistrarV1")]
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
                var respuestaAutenticacion = await UserUtils.ConstruirToken(credencialesUsuarioDTO, _configuration, _userManager, usuario.Id);
                await _servicioLlaves.CrearLlave(usuario.Id, TipoLlave.Gratuita);
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

        [HttpPost("login", Name = "LoginV1")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(
            CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario is null)
            {
                return ModelState.RetornarLoginIncorrecto();   
            }

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO!.Password!, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await UserUtils.ConstruirToken(credencialesUsuarioDTO, _configuration, _userManager, usuario.Id);
                return respuestaAutenticacion;
            }
            else
            {
                return ModelState.RetornarLoginIncorrecto();
            }
        }

        [HttpGet(Name = "ObtenerUsuariosV1")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            var usuarios = await _context.Users.ToListAsync();
            var usuariosDTO = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
            return Ok(usuariosDTO);
        }

        [HttpPut(Name = "ActualizarUsuarioV1")]
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

        [HttpGet("renovar-token", Name = "RenovarTokenV1")]
        [Authorize]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            var usuario = await _servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }
            var credencialesUsuarioDTO = new CredencialesUsuarioDTO { Email = usuario.Email! };

            var respuestaAutenticacion = await UserUtils.ConstruirToken(credencialesUsuarioDTO, _configuration, _userManager, usuario.Id);
            return respuestaAutenticacion;
        }

        [HttpPost("hacer-admin", Name = "HacerAdminV1")]
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

        [HttpPost("quitar-admin", Name = "QuitarAdminV1")]
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
    }
}
