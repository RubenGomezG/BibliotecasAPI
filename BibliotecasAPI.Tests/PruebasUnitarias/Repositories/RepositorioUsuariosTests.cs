using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Repositories
{
    [TestClass]
    public class RepositorioUsuariosTests : BasePruebas
    {
        private string nombreBD = Guid.NewGuid().ToString();
        private UserManager<Usuario> userManager = null!;
        private IHttpContextAccessor httpContextAccessor = null!;
        private IRepositorioUsuarios repositorioUsuarios = null!;
        private IConfiguration configuration = null!;
        private SignInManager<Usuario> signInManager = null!;
        private IServicioUsuarios servicioUsuarios = null!;
        private IMapper mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();

            Dictionary<string, string> configuracion = new Dictionary<string, string>
            {
                {
                    "llavejwt", "asdasdasdaasdasdasd1214124aqsasgasgasgsd"
                }
            };

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configuracion!)
                .Build();

            servicioUsuarios = Substitute.For<IServicioUsuarios>();
            ApplicationDbContext context = ConstruirContext(nombreBD);
            mapper = Substitute.For<IMapper>();
            userManager = Substitute.For<UserManager<Usuario>>(
                Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);


            var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<Usuario>>();

            signInManager = Substitute.For<SignInManager<Usuario>>(userManager,httpContextAccessor, claimsFactory, null, null, null, null);
            repositorioUsuarios = new RepositorioUsuarios(userManager, httpContextAccessor,
                configuration, signInManager, servicioUsuarios, context, mapper);
        }

        [TestMethod]
        public async Task ObtenerUsuario_RetornaNull_CuandoNoHayClaimEmail()
        {
            //Preparación
            DefaultHttpContext httpContext = new DefaultHttpContext();
            httpContextAccessor.HttpContext.Returns(httpContext);

            //Prueba
            Usuario? usuario = await repositorioUsuarios.ObtenerUsuario();

            //Verificación

            Assert.IsNull(usuario);

        }

        [TestMethod]
        public async Task ObtenerUsuario_RetornaUsuario_CuandoHayClaimEmail()
        {
            //Preparación
            string email = "rosi@almudena.com";
            Usuario usuarioEsperado = new Usuario { Email = email };
            userManager.FindByEmailAsync(email)!.Returns(Task.FromResult(usuarioEsperado));

            ClaimsPrincipal claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("email", email)
            }));

            DefaultHttpContext httpContext = new DefaultHttpContext() { User = claims };
            httpContextAccessor.HttpContext.Returns(httpContext);

            //Prueba
            Usuario? usuario = await repositorioUsuarios.ObtenerUsuario();

            //Verificación

            Assert.IsNotNull(usuario);
            Assert.AreEqual(email, usuario.Email);
        }

        [TestMethod]
        public async Task Registrar_DevuelveToken_SiEsExito()
        {
            //Preparación
            CredencialesUsuarioDTO credenciales = new CredencialesUsuarioDTO
            {
                Email = "prueba@hotmail.com",
                Password = "aA1234561234124124124!"
            };

            userManager.CreateAsync(Arg.Any<Usuario>(), Arg.Any<string>())
                .Returns(IdentityResult.Success);

            //Prueba
            ActionResult<RespuestaAutenticacionDTO> respuesta = await repositorioUsuarios.Registrar(credenciales);

            //Verificación
            Assert.IsNotNull(respuesta.Value);
        }

        [TestMethod]
        public async Task Registrar_DevuelveValidationProblem_SiFalla()
        {
            //Preparación
            string mensajeDeError = "prueba";
            CredencialesUsuarioDTO credenciales = new CredencialesUsuarioDTO
            {
                Email = "prueba@hotmail.com",
                Password = "aA123456!"
            };

            userManager.CreateAsync(Arg.Any<Usuario>(), Arg.Any<string>())
                .Returns(IdentityResult.Failed(new IdentityError
                {
                    Code = mensajeDeError,
                    Description = mensajeDeError,
                }));

            //Prueba
            try
            {
                ActionResult<RespuestaAutenticacionDTO> respuesta = await repositorioUsuarios.Registrar(credenciales);
            }
            catch (ValidationException ex)
            {
                Assert.IsNotNull(ex);
            }
        }

        [TestMethod]
        public async Task Login_DevuelveToken_SiLoginEsCorrecto()
        {
            //Preparación
            CredencialesUsuarioDTO credenciales = new CredencialesUsuarioDTO
            {
                Email = "prueba@hotmail.com",
                Password = "aA1234561234124124124!"
            };

            Usuario usuario = new Usuario
            {
                Email = credenciales.Email
            };

            userManager.FindByEmailAsync(credenciales.Email)!
                .Returns(Task.FromResult<Usuario>(usuario));

            signInManager.CheckPasswordSignInAsync(usuario, credenciales.Password, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            //Prueba
            ActionResult<RespuestaAutenticacionDTO> respuesta = await repositorioUsuarios.Login(credenciales);

            //Verificación
            Assert.IsNotNull(respuesta.Value);
        }

        [TestMethod]
        public async Task Login_DevuelveLoginIncorrecto_SiUsuarioNoExiste()
        {
            //Preparación
            string mensajeDeError = "Login incorrecto.";
            CredencialesUsuarioDTO credenciales = new CredencialesUsuarioDTO
            {
                Email = "prueb123a@hotmail.com",
                Password = "aA123456!"
            };

            userManager.FindByEmailAsync(credenciales.Email)!
                .Returns(Task.FromResult<Usuario>(null!));

            //Prueba
            ActionResult<RespuestaAutenticacionDTO> respuesta = await repositorioUsuarios.Login(credenciales);

            //Verificación
            StatusCodeResult? resultado = respuesta.Result as StatusCodeResult;
            
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Login_DevuelveLoginIncorrecto_CuandoLoginIncorrecto()
        {
            //Preparación
            string mensajeDeError = "Login incorrecto.";
            CredencialesUsuarioDTO credenciales = new CredencialesUsuarioDTO
            {
                Email = "prueb123a@hotmail.com",
                Password = "aA123456!"
            };

            Usuario usuario = new Usuario
            {
                Email = credenciales.Email
            };

            userManager.FindByEmailAsync(credenciales.Email)!
                .Returns(Task.FromResult<Usuario>(null!));

            signInManager.CheckPasswordSignInAsync(usuario, credenciales.Password, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            //Prueba
            ActionResult<RespuestaAutenticacionDTO> respuesta = await repositorioUsuarios.Login(credenciales);

            //Verificación
            StatusCodeResult? resultado = respuesta.Result as StatusCodeResult;

            Assert.AreEqual(404, resultado!.StatusCode);
        }
    }
}
