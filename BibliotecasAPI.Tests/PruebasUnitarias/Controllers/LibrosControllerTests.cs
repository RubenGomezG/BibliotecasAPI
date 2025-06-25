using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.Controllers.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.Tests.TestUtils;
using NSubstitute;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Controllers
{
    [TestClass]
    public class LibrosControllerTests : BasePruebas
    {
        private IServicioLibros servicioLibros = null!;
        private LibrosController controller = null!;

        [TestInitialize]
        public void Setup()
        {
            servicioLibros = Substitute.For<IServicioLibros>();
            controller = new LibrosController(servicioLibros);
        }
        
        [TestMethod]
        public async Task Get_LlamaAlServicioCorrectamente()
        {
            PaginacionDTO paginacionDTO = new PaginacionDTO(1, 1);
            //Prueba
            IEnumerable<LibroConAutoresDTO> respuesta = await controller.Get(paginacionDTO);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //Verificación                   
            Assert.AreEqual(0, respuesta.Count());
            await servicioLibros.Received(1).GetLibros(paginacionDTO);
        }
    }
}
