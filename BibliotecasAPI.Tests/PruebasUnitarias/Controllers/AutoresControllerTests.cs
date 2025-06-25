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
        private string nombreBD = Guid.NewGuid().ToString();
        private IServicioAutores servicioAutores = null!;
        private AutoresController controller = null!;
        IOutputCacheStore outputCacheStore = null!;
        private const string CACHE_AUTORES = "autores-obtener";

        [TestInitialize]
        public void Setup()
        {
            ApplicationDbContext context = ConstruirContext(nombreBD);
            IMapper mapper = ConfigurarAutoMapper();
            servicioAutores = Substitute.For<IServicioAutores>();
            outputCacheStore = Substitute.For<IOutputCacheStore>();
            controller = new AutoresController(context, mapper, servicioAutores, outputCacheStore);
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

        [TestMethod]
        public async Task Patch_Retorna400_CuandoPatchDocEsNulo()
        {
            //Prueba
            ActionResult respuesta = await controller.Patch(1, null!);

            //Verificación

            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(400, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Patch_Retorna404_CuandoAutorNoExiste()
        {
            //Preparación
            JsonPatchDocument<AutorPatchDTO> patchDoc = new JsonPatchDocument<AutorPatchDTO>();

            //Prueba
            ActionResult respuesta = await controller.Patch(1, patchDoc);

            //Verificación
            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Patch_RetornaValidationProblem_CuandoHayErrorPoder()
        {
            //Preparación
            ApplicationDbContext context = ConstruirContext(nombreBD);
            context.Autores.Add(new Autor
            {
                Nombre = "Rosi",
                Apellidos = "Rosez",
                Identificacion = "123"
            });

            await context.SaveChangesAsync();

            IObjectModelValidator objectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator = objectValidator;

            string mensajeDeError = "mensaje de error";
            controller.ModelState.AddModelError("", mensajeDeError);
            JsonPatchDocument<AutorPatchDTO> patchDoc = new JsonPatchDocument<AutorPatchDTO>();
            //Prueba
            ActionResult respuesta = await controller.Patch(1, patchDoc);

            //Verificación

            ObjectResult? resultado = respuesta as ObjectResult;
            ValidationProblemDetails? problemDetails = resultado!.Value as ValidationProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(1, problemDetails.Errors.Keys.Count);
            Assert.AreEqual(mensajeDeError, problemDetails.Errors.Values.First().First());
        }

        [TestMethod]
        public async Task Patch_ActualizaUnSoloCampo_CuandoSeLeEnviaUnaOperacionConUnSoloCambio()
        {
            //Preparación
            ApplicationDbContext context = ConstruirContext(nombreBD);

            context.Autores.Add(new Autor
            {
                Nombre = "Rosi",
                Apellidos = "Rosez",
                Identificacion = "123",
                Foto = "URL-1"
            });

            await context.SaveChangesAsync();

            IObjectModelValidator objectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator = objectValidator;

            JsonPatchDocument<AutorPatchDTO> patchDoc = new JsonPatchDocument<AutorPatchDTO>();
            patchDoc.Operations.Add(new Operation<AutorPatchDTO>("replace", "/nombre", null, "Rosiiiiiii"));

            //Prueba
            ActionResult respuesta = await controller.Patch(1, patchDoc);

            //Verificación
            StatusCodeResult? resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado!.StatusCode);
            await outputCacheStore.Received(1).EvictByTagAsync(CACHE_AUTORES, default);

            ApplicationDbContext context2 = ConstruirContext(nombreBD);
            Autor autorBD = await context2.Autores.SingleAsync();

            Assert.AreEqual(expected: "Rosiiiiiii", autorBD.Nombre);
            Assert.AreEqual(expected: "Rosez", autorBD.Apellidos);
            Assert.AreEqual(expected: "123", autorBD.Identificacion);
            Assert.AreEqual(expected: "URL-1", actual: autorBD.Foto);
        }
    }
}
