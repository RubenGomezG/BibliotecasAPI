using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Utils.ClassUtils
{
    public static class LlaveApiUtils
    {
        public static string GenerarLlave() => Guid.NewGuid().ToString().Replace("-", "");

        public static LlaveAPI CrearLlave(ApplicationDbContext context, string usuarioId, TipoLlave tipoLlave)
        {
            var llave = GenerarLlave();

            var llaveAPI = new LlaveAPI
            {
                Llave = llave,
                TipoLlave = tipoLlave,
                Activa = true,
                UsuarioId = usuarioId
            };

            return llaveAPI;
        }
    }
}
