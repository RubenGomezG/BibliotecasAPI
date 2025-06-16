using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.Tests.TestUtils;
using System.Net;

namespace BibliotecasAPI.Tests.PruebasIntegracion.Controllers
{
    [TestClass]
    public class LibrosControllerIntegrationTests : BasePruebas
    {
        private readonly string url = "/api/v1/libros";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Post_Devuelve_400_CuandoAutoresidsEsVacio()
        {
            //Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();
            var libroCreacionDTo = new LibroCreacionDTO
            {
                Titulo = "Titulo"
            };

            //Prueba
            var respuesta = await cliente.PostAsJsonAsync(url, libroCreacionDTo);

            //Verificación
            Assert.AreEqual(HttpStatusCode.BadRequest, respuesta.StatusCode);
        }
    }
}
