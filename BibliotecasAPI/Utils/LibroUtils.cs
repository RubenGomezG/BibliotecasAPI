using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Utils
{
    public static class LibroUtils
    {
        public async static Task<bool> ExisteLibro(ApplicationDbContext context, int libroId)
        {
            return await context.Libros.AnyAsync(l => l.Id == libroId);
        }

        public static void AsignarOrdenAutores(Libro libro)
        {
            if (libro.Autores != null)
            {
                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Orden = i;
                }
            }
        }
    }
}
