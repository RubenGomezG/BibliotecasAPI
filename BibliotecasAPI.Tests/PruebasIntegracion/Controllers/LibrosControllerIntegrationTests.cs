using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace BibliotecasAPI.Tests.PruebasIntegracion.Controllers
{
    [TestClass]
    public class LibrosControllerIntegrationTests : BasePruebas
    {
        private readonly string url = "/api/v1/libros";
        private readonly string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Post_Devuelve_400_CuandoAutoresidsEsVacio()
        {
            //Preparación
            WebApplicationFactory<Program> factory = ConstruirWebApplicationFactory(nombreBD);
            HttpClient cliente = factory.CreateClient();
            LibroCreacionDTO libroCreacionDTo = new LibroCreacionDTO
            {
                Titulo = "Titulo"
            };

            //Prueba
            HttpResponseMessage respuesta = await cliente.PostAsJsonAsync(url, libroCreacionDTo);

            //Verificación
            Assert.AreEqual(HttpStatusCode.BadRequest, respuesta.StatusCode);
        }
    }
}
