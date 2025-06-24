using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace BibliotecasAPI.Tests.PruebasIntegracion.Controllers
{
    [TestClass]
    public class AutoresControllerIntegrationTests : BasePruebas
    {
        private static readonly string url = "/api/v1/autores";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Post_Devuelve401_CuandoUsuarioNoAutenticado()
        {
            //Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD, false);
            var cliente = factory.CreateClient();
            var autorCreacionDTO = new AutorCreacionDTO
            {
                Nombre = "Yo",
                Apellidos = "Tu",
                Identificacion = "123"
            };

            //Prueba
            var respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);

            //Verificación
            Assert.AreEqual(HttpStatusCode.Unauthorized, respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Post_Devuelve403_CuandoUsuarioNoEsAdmin()
        {
            //Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD, false);
            var token = await CrearUsuario(nombreBD, factory);
            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var autorCreacionDTO = new AutorCreacionDTO
            {
                Nombre = "Yo",
                Apellidos = "Tu",
                Identificacion = "123"
            };

            //Prueba
            var respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);

            //Verificación
            Assert.AreEqual(HttpStatusCode.Forbidden, respuesta.StatusCode);
        }
    }
}
