using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.BLL.Services.Impl.V1;
using BibliotecasAPI.BLL.Services.Interfaces;
using BibliotecasAPI.Controllers.V1;
using BibliotecasAPI.DAL.Datos;
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
using System.Net;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Repositories
{
    [TestClass]
    public class RepositorioAutoresTests : BasePruebas
    {
        IAlmacenadorArchivos almacenadorArchivos = null!;
        IHttpContextAccessor httpContextAccessor = null!;
        ILogger<RepositorioAutores> logger = null!;
        IOutputCacheStore outputCacheStore = null!;
        //private AutoresController controller = null!;
        private string nombreBD = Guid.NewGuid().ToString();
        private RepositorioAutores repositorio = null!;
        private const string CACHE_AUTORES = "autores-obtener";
        private const string CONTENEDOR = "autores";

        [TestInitialize] 
        public void Setup()
        {
            ApplicationDbContext context = ConstruirContext(nombreBD);
            IMapper mapper = ConfigurarAutoMapper();
            almacenadorArchivos = Substitute.For<IAlmacenadorArchivos>();
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            logger = Substitute.For<ILogger<RepositorioAutores>>();
            outputCacheStore = Substitute.For<IOutputCacheStore>();
            //controller = new AutoresController(servicioAutores);
            repositorio = new RepositorioAutores(context, httpContextAccessor, mapper, outputCacheStore, logger, almacenadorArchivos);
        }

        [TestMethod]
        public async Task Get_Retorna404_CuandoAutorConIdNoExiste() 
        {
            //Prueba
            var respuesta = await repositorio.GetAutorPorId(1);
            var statusCode = respuesta.Result as StatusCodeResult;

            //Verificación                   
            Assert.AreEqual(404, statusCode!.StatusCode);
        }

        [TestMethod]
        public async Task GetAutorPorId_RetornaAutor_CuandoAutorConIdExiste()
        {
            //Preparación
            ApplicationDbContext context = ConstruirContext(nombreBD);

            context.Autores.Add(new Autor { Nombre = "Yo", Apellidos = "Tu El" });
            context.Autores.Add(new Autor { Nombre = "Nosotros", Apellidos = "Vosotros Ellos" });

            await context.SaveChangesAsync();

            //Prueba
            ActionResult<AutorConLibrosDTO> respuesta = await repositorio.GetAutorPorId(1);
            ApplicationDbContext context2 = ConstruirContext(nombreBD);
            Autor autorIntroducido1 = await context2.Autores.SingleAsync(x => x.Id == 1);
            //Verificación            
            Assert.AreEqual(expected: "Yo", actual: autorIntroducido1.Nombre);
        }

        [TestMethod]
        public async Task GetAutorPorId_RetornaAutorConLibros_CuandoAutorTieneLibros()
        {
            //Preparación            
            ApplicationDbContext context = ConstruirContext(nombreBD);

            Libro libro1 = new Libro { Titulo = "Libro 1" };
            Libro libro2 = new Libro { Titulo = "Libro 2" };
            context.Libros.Add(libro1);
            context.Libros.Add(libro2);
            context.Autores.Add(new Autor
            {
                Nombre = "Autor",
                Apellidos = "Con Libros",
                Libros = new List<AutorLibro>
                {
                    new AutorLibro {Libro = libro1},
                    new AutorLibro {Libro = libro2}
                }
            });

            await context.SaveChangesAsync();
            //Prueba
            ActionResult<AutorConLibrosDTO> respuesta = await repositorio.GetAutorPorId(1);
            ApplicationDbContext context2 = ConstruirContext(nombreBD);
            Autor autorIntroducido1 = await context2.Autores.Include(x => x.Libros).SingleAsync(x => x.Id == 1);

            //Verificación
            Assert.AreEqual(expected: 1, actual: autorIntroducido1.Id);
            Assert.AreEqual(expected: 2, actual: autorIntroducido1.Libros.Count);
        }

        [TestMethod]
        public async Task AnadirAutor_DebeCrearAutor_CuandoEnviamosAutor()
        {
            //Prepare
            ApplicationDbContext context = ConstruirContext(nombreBD);
            AutorCreacionDTO nuevoAutor = new AutorCreacionDTO
            { 
                Nombre = "Nuevo",
                Apellidos = "Autor"
            };

            //Prueba
            ActionResult respuesta = await repositorio.AnadirAutor(nuevoAutor);

            //Verificación
            CreatedAtRouteResult? resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            int cantidad = await context.Autores.CountAsync();
            Assert.AreEqual(expected: 1, actual: cantidad);
        }

        [TestMethod]
        public async Task ActualizarAutor_Retorna404_CuandoAutorNoExiste()
        {
            //Prueba
            ActionResult respuesta = await repositorio.ActualizarAutor(1, autorCreacionDTO: null!);

            //Verificación

            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task ActualizarAutor_Actualiza_CuandoEnviamosAutorSinFoto()
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

            AutorCreacionConFotoDTO autorCreacionDTO = new AutorCreacionConFotoDTO
            {
                Nombre = "Rosi2",
                Apellidos = "Rosez2",
                Identificacion = "1234"
            };

            //Prueba
            ActionResult respuesta = await repositorio.ActualizarAutor(1, autorCreacionDTO);

            //Verificación

            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            ApplicationDbContext context2 = ConstruirContext(nombreBD);
            Autor actorActualizado = await context2.Autores.SingleAsync();
            Assert.AreEqual(expected: "Rosi2", actorActualizado.Nombre);
            Assert.AreEqual(expected: "Rosez2", actorActualizado.Apellidos);
            Assert.AreEqual(expected: "1234", actorActualizado.Identificacion);
            await outputCacheStore.Received(1).EvictByTagAsync(CACHE_AUTORES, default);
            await almacenadorArchivos.DidNotReceiveWithAnyArgs().Editar(default, default!, default!);
        }

        [TestMethod]
        public async Task ActualizarAutor_Actualiza_CuandoEnviamosAutorConFoto()
        {
            //Preparación
            ApplicationDbContext context = ConstruirContext(nombreBD);

            string urlAnterior = "URL-1";
            string urlNueva = "URL-2";
            almacenadorArchivos.Editar(default, default!, default!).ReturnsForAnyArgs(urlNueva);
            context.Autores.Add(new Autor
            {
                Nombre = "Rosi",
                Apellidos = "Rosez",
                Identificacion = "123",
                Foto = urlAnterior
            });

            await context.SaveChangesAsync();

            IFormFile formFile = Substitute.For<IFormFile>();

            AutorCreacionConFotoDTO autorCreacionDTO = new AutorCreacionConFotoDTO
            {
                Nombre = "Rosi2",
                Apellidos = "Rosez2",
                Identificacion = "1234",
                Foto = formFile
            };

            //Prueba
            ActionResult respuesta = await repositorio.ActualizarAutor(1, autorCreacionDTO);

            //Verificación

            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            ApplicationDbContext context2 = ConstruirContext(nombreBD);
            Autor actorActualizado = await context2.Autores.SingleAsync();
            Assert.AreEqual(expected: "Rosi2", actorActualizado.Nombre);
            Assert.AreEqual(expected: "Rosez2", actorActualizado.Apellidos);
            Assert.AreEqual(expected: "1234", actorActualizado.Identificacion);
            Assert.AreEqual(expected: urlNueva, actual: actorActualizado.Foto);
            await outputCacheStore.Received(1).EvictByTagAsync(CACHE_AUTORES, default);
            await almacenadorArchivos.Received(1).Editar(urlAnterior, CONTENEDOR, formFile);
        }

        [TestMethod]
        public async Task BorrarAutor_Retorna404_CuandoAutorNoExiste()
        {
            //Prueba
            ActionResult respuesta = await repositorio.BorrarAutor(1);

            //Verificación

            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task BorrarAutor_BorraAutor_CuandoAutorExiste()
        {
            //Preparación
            ApplicationDbContext context = ConstruirContext(nombreBD);

            string urlFoto = "URL-1";

            context.Autores.Add(new Autor{ Nombre = "Rosi", Apellidos = "Rosez", Foto = urlFoto });
            context.Autores.Add(new Autor{ Nombre = "Rosi2", Apellidos = "Rosez2" });

            await context.SaveChangesAsync();
            //Prueba
            ActionResult respuesta = await repositorio.BorrarAutor(1);

            //Verificación

            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            ApplicationDbContext context2 = ConstruirContext(nombreBD);
            int cantidadAutores = await context2.Autores.CountAsync();
            Assert.AreEqual(1, cantidadAutores);

            bool autor2Existe = await context2.Autores.AnyAsync(autor => autor.Nombre == "Rosi2");
            Assert.IsTrue(autor2Existe);

            await outputCacheStore.Received(1).EvictByTagAsync(CACHE_AUTORES, default);
            await almacenadorArchivos.Received(1).Borrar(urlFoto, CONTENEDOR);
        }

        [TestMethod]
        public async Task Patch_Retorna400_CuandoPatchDocEsNulo()
        {
            //Prueba
            ActionResult respuesta = await repositorio.PatchAutor(1, null!);

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
            ActionResult respuesta = await repositorio.PatchAutor(1, patchDoc);

            //Verificación
            StatusCodeResult? resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
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

            JsonPatchDocument<AutorPatchDTO> patchDoc = new JsonPatchDocument<AutorPatchDTO>();
            patchDoc.Operations.Add(new Operation<AutorPatchDTO>("replace", "/nombre", null, "Rosiiiiiii"));

            //Prueba
            ActionResult respuesta = await repositorio.PatchAutor(1, patchDoc);

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
