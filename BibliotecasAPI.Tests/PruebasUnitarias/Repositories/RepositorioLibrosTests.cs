using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.AspNetCore.OutputCaching;
using NSubstitute;

namespace BibliotecasAPI.Tests.PruebasUnitarias.Repositories
{
    [TestClass]
    public class RepositorioLibrosTests : BasePruebas
    {
        private ApplicationDbContext context = null!;
        private IOutputCacheStore outputCacheStore = null!;
        private IMapper mapper = null!;
        private string nombreBD = Guid.NewGuid().ToString();
        private RepositorioLibros repositorio = null!;
        private IHttpContextAccessor httpContextAccessor = null!;
        private const string CACHE_LIBROS = "libros-obtener";

        [TestInitialize]
        public void Setup()
        {
            context = ConstruirContext(nombreBD);
            mapper = ConfigurarAutoMapper();
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            outputCacheStore = Substitute.For<IOutputCacheStore>();

            repositorio = new RepositorioLibros(context, mapper, httpContextAccessor, outputCacheStore);
        }

        [TestMethod]
        public async Task Get_RetornaCeroLibros_CuandoNoHayLibros()
        {
            //Preparación
            ApplicationDbContext context = ConstruirContext(nombreBD);
            PaginacionDTO paginacionDTO = new PaginacionDTO(1, 1);
            //Prueba
            IEnumerable<LibroConAutoresDTO> respuesta = await repositorio.GetLibros(paginacionDTO);
            httpContextAccessor.HttpContext.Returns(new DefaultHttpContext());

            //Verificación                   
            Assert.AreEqual(0, respuesta.Count());
        }
    }
}
