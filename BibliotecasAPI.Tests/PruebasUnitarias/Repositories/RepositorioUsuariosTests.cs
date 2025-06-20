using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            var httpContext = new DefaultHttpContext();
            httpContextAccessor.HttpContext.Returns(httpContext);

            //Prueba
            var usuario = await repositorioUsuarios.ObtenerUsuario();

            //Verificación

            Assert.IsNull(usuario);

        }

        [TestMethod]
        public async Task ObtenerUsuario_RetornaUsuario_CuandoHayClaimEmail()
        {
            //Preparación
            var email = "rosi@almudena.com";
            var usuarioEsperado = new Usuario { Email = email };
            userManager.FindByEmailAsync(email)!.Returns(Task.FromResult(usuarioEsperado));

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("email", email)
            }));

            var httpContext = new DefaultHttpContext() { User = claims};
            httpContextAccessor.HttpContext.Returns(httpContext);

            //Prueba
            var usuario = await repositorioUsuarios.ObtenerUsuario();

            //Verificación

            Assert.IsNotNull(usuario);
            Assert.AreEqual(email, usuario.Email);
        }
    }
}
