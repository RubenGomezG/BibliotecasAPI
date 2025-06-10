using AutoMapper;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.Utils.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecasAPI.Tests.TestUtils
{
    public class BasePruebas
    {
        protected ApplicationDbContext ConstruirContext(string nombreBBDD)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(nombreBBDD).Options;
            var dbContext = new ApplicationDbContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(opciones =>
            {
                opciones.AddProfile(new AutoMapperProfiles());
            });
            return config.CreateMapper();
        }
    }
}
