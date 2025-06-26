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
        private SignInManager<Usuario> signInManager = null!;
        private UsuariosController controller = null!;
        private IServicioUsuarios servicioUsuarios = null!;

        [TestInitialize]
        public void Setup()
        {
            ApplicationDbContext context = ConstruirContext(nombreBD);

            userManager = Substitute.For<UserManager<Usuario>>(
                Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

            Dictionary<string, string> configuracion = new Dictionary<string, string>
            {
                {
                    "llavejwt", "asdasdasdaasdasdasd1214124aqsasgasgasgsd"
                }
            };


            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            servicioUsuarios = Substitute.For<IServicioUsuarios>();
            IUserClaimsPrincipalFactory<Usuario> userClaimsFactory = Substitute.For<IUserClaimsPrincipalFactory<Usuario>>();
            signInManager = Substitute.For<SignInManager<Usuario>>(userManager, httpContextAccessor,
                userClaimsFactory, null, null, null, null);

            controller = new UsuariosController(servicioUsuarios);
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

            servicioUsuarios.Login(credenciales).Returns(new NotFoundResult());

            //Prueba
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Login(credenciales);

            //Verificación
            StatusCodeResult? resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(mensajeDeError, controller.ModelState.Root.Errors.First().ErrorMessage);
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
            servicioUsuarios.Login(credenciales).Returns(new NotFoundResult());

            //Prueba
            ActionResult<RespuestaAutenticacionDTO> respuesta = await controller.Login(credenciales);

            //Verificación
            StatusCodeResult? resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(mensajeDeError, controller.ModelState.Root.Errors.First().ErrorMessage);
        }
    }
}
