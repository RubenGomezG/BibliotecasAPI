using AutoMapper;
using BibliotecasAPI.DAL.Datos;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace BibliotecasAPI.Utils
{
    public static class LibroUtils
    {
        public async static Task<bool> ExisteLibro(ApplicationDbContext context, int libroId)
        {
            return await context.Libros.AnyAsync(l => l.Id == libroId);
        }
    }
}
