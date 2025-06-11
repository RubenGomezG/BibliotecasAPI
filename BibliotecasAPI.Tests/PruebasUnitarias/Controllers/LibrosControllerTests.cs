using BibliotecasAPI.BLL.Services.Impl.V1;
using BibliotecasAPI.BLL.Services.Impl.V2;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.Controllers.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var paginacionDTO = new PaginacionDTO(1, 1);
            //Prueba
            var respuesta = await controller.Get(paginacionDTO);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //Verificación                   
            Assert.AreEqual(0, respuesta.Count());
            await servicioLibros.Received(1).GetLibros(paginacionDTO);
        }
    }
}
