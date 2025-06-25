using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.Controllers.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Controllers
{
    [TestClass]
    public class AutoresControllerTests : BasePruebas
    {
        private IServicioAutores servicioAutores = null!;
        private AutoresController controller = null!;

        [TestInitialize]
        public void Setup()
        {
            servicioAutores = Substitute.For<IServicioAutores>();
            controller = new AutoresController(servicioAutores);
        }
        [TestMethod]
        public async Task Get_DebeLlamarServicioAutores()
        {
            //Prepare
            PaginacionDTO paginacionDTO = new PaginacionDTO(2, 3);

            //Action
            await controller.Get(paginacionDTO);

            //Assert
            await servicioAutores.Received(1).GetAutores(paginacionDTO);
        }

        
    }
}
