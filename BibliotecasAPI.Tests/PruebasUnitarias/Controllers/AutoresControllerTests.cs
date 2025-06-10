using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.BLL.Services.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.Controllers.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.Tests.TestUtils;
using BibliotecasAPI.Tests.TestUtils.Dobles;
using Microsoft.AspNetCore.Http;
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
    public class AutoresControllerTests : BasePruebas
    {
        private string nombreBD = Guid.NewGuid().ToString();
        private IServicioAutores servicioAutores = null!;
        private AutoresController controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            servicioAutores = Substitute.For<IServicioAutores>();
            controller = new AutoresController(context, mapper, servicioAutores);
        }
        [TestMethod]
        public async Task Get_DebeLlamarServicioAutores()
        {
            //Prepare
            var paginacionDTO = new PaginacionDTO(2, 3);

            //Action
            await controller.Get(paginacionDTO);

            //Assert
            await servicioAutores.Received(1).GetAutores(paginacionDTO);

        }
    }
}
