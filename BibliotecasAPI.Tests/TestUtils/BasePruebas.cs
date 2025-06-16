using AutoMapper;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.Utils.MappingProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;
using System.Text.Json;

namespace BibliotecasAPI.Tests.TestUtils
{
    public class BasePruebas
    {
        protected readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
        protected readonly Claim esAdmin = new Claim("esAdmin", "1");

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

        protected WebApplicationFactory<Program> ConstruirWebApplicationFactory(string nombreBD, bool ignorarSeguridad = true)
        {
            var factory = new WebApplicationFactory<Program>();

            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    ServiceDescriptor descriptorDBContext = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))!;

                    if (descriptorDBContext != null)
                    {
                        services.Remove(descriptorDBContext);
                    }

                    services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseInMemoryDatabase(nombreBD));

                    if (ignorarSeguridad)
                    {
                        services.AddSingleton<IAuthorizationHandler, AlloyAnonymousHandler>();

                        services.AddControllers(opciones =>
                        {
                            opciones.Filters.Add(new DummyUserFilter());
                        });
                    }
                });
            });

            return factory;
        }

        protected async Task<string> CrearUsuario(string nombreBD, WebApplicationFactory<Program> factory) =>
            await CrearUsuario(nombreBD, factory, [], "ejemplo@hotmail.com");

        protected async Task<string> CrearUsuario(string nombreBD, WebApplicationFactory<Program> factory, IEnumerable<Claim> claims) =>
            await CrearUsuario(nombreBD, factory, claims, "ejemplo@hotmail.com");

        protected async Task<string> CrearUsuario(string nombreBD, WebApplicationFactory<Program> factory, IEnumerable<Claim> claims, string email)
        {
            var urlRegistro = "/api/v1/usuarios/registro";
            string token = string.Empty;
            token = await ConstruirToken(email, urlRegistro, factory);

            if (claims.Any())
            {
                var context = ConstruirContext(nombreBD);
                var usuario = await context.Users.Where(user => user.Email == email).FirstAsync();
                Assert.IsNotNull(usuario);

                var userClaims = claims.Select(x => new IdentityUserClaim<string>
                {
                    UserId = usuario.Id,
                    ClaimType = x.Type,
                    ClaimValue = x.Value
                });

                context.UserClaims.AddRange(userClaims);
                await context.SaveChangesAsync();
                var urlLogin = "/api/v1/usuarios/login";
                token = await ConstruirToken(email, urlLogin, factory);
            }

            return token;
        }

        private async Task<string> ConstruirToken(string email, string url, WebApplicationFactory<Program> factory)
        {
            var password = "aA1234561234124124124!";
            var credenciales = new CredencialesUsuarioDTO { Email = email, Password = password };
            var cliente = factory.CreateClient();
            var respuesta = await cliente.PostAsJsonAsync(url, credenciales);

            respuesta.EnsureSuccessStatusCode();
            var contenido = await respuesta.Content.ReadAsStringAsync();
            var respuestaAutenticacion = JsonSerializer.Deserialize<RespuestaAutenticacionDTO>(contenido, jsonSerializerOptions)!;

            Assert.IsNotNull(respuestaAutenticacion!.Token);

            return respuestaAutenticacion.Token;


        }
    }
}
