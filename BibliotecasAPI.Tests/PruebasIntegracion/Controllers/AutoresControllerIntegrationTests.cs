using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;

namespace BibliotecasAPI.Tests.PruebasIntegracion.Controllers
{
    [TestClass]
    public class AutoresControllerIntegrationTests : BasePruebas
    {
        private static readonly string url = "/api/v1/autores";
        private readonly string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Post_Devuelve401_CuandoUsuarioNoAutenticado()
        {
            //Preparación
            WebApplicationFactory<Program> factory = ConstruirWebApplicationFactory(nombreBD, false);
            HttpClient cliente = factory.CreateClient();
            AutorCreacionDTO autorCreacionDTO = new AutorCreacionDTO
            {
                Nombre = "Yo",
                Apellidos = "Tu",
                Identificacion = "123"
            };

            //Prueba
            HttpResponseMessage respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);

            //Verificación
            Assert.AreEqual(HttpStatusCode.Unauthorized, respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Post_Devuelve403_CuandoUsuarioNoEsAdmin()
        {
            //Preparación
            WebApplicationFactory<Program> factory = ConstruirWebApplicationFactory(nombreBD, false);
            string token = await CrearUsuario(nombreBD, factory);
            HttpClient cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            AutorCreacionDTO autorCreacionDTO = new AutorCreacionDTO
            {
                Nombre = "Yo",
                Apellidos = "Tu",
                Identificacion = "123"
            };

            //Prueba
            HttpResponseMessage respuesta = await cliente.PostAsJsonAsync(url, autorCreacionDTO);

            //Verificación
            Assert.AreEqual(HttpStatusCode.Forbidden, respuesta.StatusCode);
        }
    }
}
