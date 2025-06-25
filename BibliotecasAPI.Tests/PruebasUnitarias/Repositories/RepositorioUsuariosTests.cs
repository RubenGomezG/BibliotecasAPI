using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System.Security.Claims;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Repositories
{
    [TestClass]
    public class RepositorioUsuariosTests : BasePruebas
    {
        private UserManager<Usuario> userManager = null!;
        private IHttpContextAccessor httpContextAccessor = null!;
        private IRepositorioUsuarios repositorioUsuarios = null!;

        [TestInitialize]
        public void Setup()
        {
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            userManager = Substitute.For<UserManager<Usuario>>(
                Substitute.For<IUserStore<Usuario>>(), null, null, null, null, null, null, null, null);

            repositorioUsuarios = new RepositorioUsuarios(userManager, httpContextAccessor);
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
    }
}
