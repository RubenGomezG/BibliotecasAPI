using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Utils.Validation
{
    public static class LlaveApiUtils
    {
        public static ActionResult ValidarLlaveAPI(IServicioUsuarios servicioUsuarios, LlaveAPI? llaveDB)
        {
            if (llaveDB == null)
            {
                return new NotFoundResult();
            }

            string? usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (llaveDB.UsuarioId != usuarioId)
            {
                return new ForbidResult();
            }

            return new NoContentResult();
        }
    }
}
