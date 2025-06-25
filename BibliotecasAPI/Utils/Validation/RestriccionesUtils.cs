using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Utils.Validation
{
    public static class RestriccionesUtils
    {
        public static ActionResult ValidarRestriccionDominio(IServicioUsuarios servicioUsuarios, RestriccionDominio? restriccionDB)
        {
            if (restriccionDB == null)
            {
                return new NotFoundResult();
            }

            string? usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (restriccionDB.Llave!.UsuarioId != usuarioId)
            {
                return new ForbidResult();
            }

            return new NoContentResult();
        }

        public static ActionResult ValidarRestriccionIp(IServicioUsuarios servicioUsuarios, RestriccionIp? restriccionDB)
        {
            if (restriccionDB == null)
            {
                return new NotFoundResult();
            }

            string? usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (restriccionDB.Llave!.UsuarioId != usuarioId)
            {
                return new ForbidResult();
            }

            return new NoContentResult();
        }
    }
}
