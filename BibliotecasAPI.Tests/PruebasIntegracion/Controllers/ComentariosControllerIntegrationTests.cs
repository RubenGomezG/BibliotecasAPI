using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Tests.TestUtils;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BibliotecasAPI.Tests.PruebasIntegracion.Controllers
{
    [TestClass]
    public class ComentariosControllerIntegrationTests : BasePruebas
    {
        private readonly string url = "/api/v1/libros/1/comentarios";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task BorrarComentario_Devuelve204_CuandoUsuarioBorraSuComentario()
        {
            //Preparación
            await CrearDatosDePrueba();

            var factory = ConstruirWebApplicationFactory(nombreBD, false);
            var token = await CrearUsuario(nombreBD,factory);
            var context = ConstruirContext(nombreBD);

            var usuario = await context.Users.FirstAsync();
            var comentario = new Comentario
            {
                Cuerpo = "La verdad que sí, soy la más bonita del mundo y Rosi la segunda",
                UsuarioId = usuario!.Id,
                LibroId = 1
            };
            context.Add(comentario);
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            //Prueba
            var respuesta = await cliente.DeleteAsync($"{url}/{comentario.Id}");

            //Verificación
            Assert.AreEqual(HttpStatusCode.NoContent, respuesta.StatusCode);
        }

        [TestMethod]
        public async Task BorrarComentario_Devuelve403_CuandoUsuarioIntentaBorrarComentarioDeOtroUsuario()
        {
            //Preparación
            await CrearDatosDePrueba();

            var factory = ConstruirWebApplicationFactory(nombreBD, false);
            string emailCreadorComentario = "creador-comentario@hotmail.com";
            await CrearUsuario(nombreBD, factory, [], emailCreadorComentario);
            var context = ConstruirContext(nombreBD);
            var usuarioCreadorComentario = await context.Users.FirstAsync();

            var comentario = new Comentario
            {
                Cuerpo = "La verdad que sí, soy la más bonita del mundo y Rosi la segunda",
                UsuarioId = usuarioCreadorComentario!.Id,
                LibroId = 1
            };
            context.Add(comentario);
            await context.SaveChangesAsync();

            string emailUsuarioDistinto = "usuario-distinto@hotmail.com";
            var tokenUsuarioDistinto = await CrearUsuario(nombreBD, factory, [], emailUsuarioDistinto);

            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenUsuarioDistinto);

            //Prueba
            var respuesta = await cliente.DeleteAsync($"{url}/{comentario.Id}");

            //Verificación
            Assert.AreEqual(HttpStatusCode.Forbidden, respuesta.StatusCode);
        }


        private async Task CrearDatosDePrueba()
        {
            var context = ConstruirContext(nombreBD);
            var autor = new Autor { Nombre = "Almudena", Apellidos = "La más bonita" };
            context.Add(autor);
            await context.SaveChangesAsync();

            var libro = new Libro { Titulo = "Soy Guapisima" };
            libro.Autores.Add(new AutorLibro { Autor = autor });
            context.Add(libro);
            await context.SaveChangesAsync();
        }
    }
}
