using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.Controllers.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Controllers
{
    [TestClass]
    public class UsuariosControllerTests : BasePruebas
    {
        private string nombreBD = Guid.NewGuid().ToString();
        private UserManager<Usuario> userManager = null!;
        private IHttpContextAccessor httpContextAccessor = null!;
        private IConfiguration configuration = null!;
        private SignInManager<Usuario> signInManager = null!;
        private UsuariosController controller = null!;
        private IServicioUsuarios servicioUsuarios = null!;
        private IMapper mapper = null!;

        [TestInitialize]
        public void Setup()
        {
            ApplicationDbContext context = ConstruirContext(nombreBD);
            mapper = ConfigurarAutoMapper();

            userManager = Substitute.For<UserManager<Usuario>>(
                Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

            Dictionary<string, string> configuracion = new Dictionary<string, string>
            {
                {
                    "llavejwt", "asdasdasdaasdasdasd1214124aqsasgasgasgsd"
                }
            };

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configuracion!)
                .Build();

            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            servicioUsuarios = Substitute.For<IServicioUsuarios>();
            IUserClaimsPrincipalFactory<Usuario> userClaimsFactory = Substitute.For<IUserClaimsPrincipalFactory<Usuario>>();
            signInManager = Substitute.For<SignInManager<Usuario>>(userManager, httpContextAccessor,
                userClaimsFactory, null, null, null, null);

            controller = new UsuariosController(userManager, configuration, signInManager, servicioUsuarios, context, mapper);
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
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Registrar(credenciales);

            //Verificación
            ObjectResult? resultado = respuesta.Result as ObjectResult;
            ValidationProblemDetails? problemDetails = resultado!.Value as ValidationProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(1, problemDetails.Errors.Keys.Count);
            Assert.AreEqual(mensajeDeError, problemDetails.Errors.Values.First().First());
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
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Registrar(credenciales);

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
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Login(credenciales);

            //Verificación
            ObjectResult? resultado = respuesta.Result as ObjectResult;
            ValidationProblemDetails? problemDetails = resultado!.Value as ValidationProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(1, problemDetails.Errors.Keys.Count);
            Assert.AreEqual(mensajeDeError, problemDetails.Errors.Values.First().First());
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
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Login(credenciales);

            //Verificación
            ObjectResult? resultado = respuesta.Result as ObjectResult;
            ValidationProblemDetails? problemDetails = resultado!.Value as ValidationProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(1, problemDetails.Errors.Keys.Count);
            Assert.AreEqual(mensajeDeError, problemDetails.Errors.Values.First().First());
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
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Login(credenciales);

            //Verificación
            Assert.IsNotNull(respuesta.Value);
        }
    }
}
