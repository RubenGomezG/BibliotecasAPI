using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Utils.ClassUtils
{
    public class ComentarioUtils
    {
        public static async Task<ActionResult> ValidarComentario(IServicioUsuarios servicioUsuarios, Comentario? comentarioDB)
        {
            Usuario? usuario = await servicioUsuarios.ObtenerUsuario();
            if (UserUtils.ExisteUsuario(usuario))
            {
                return new NotFoundResult();
            }

            if (ExisteComentario(comentarioDB))
            {
                return new NotFoundResult();
            }

            if (comentarioDB!.UsuarioId != usuario!.Id)
            {
                return new ForbidResult();
            }

            return new NoContentResult();
        }

        public static bool ExisteComentario(Comentario? comentario)
        {
            return comentario != null;
        }
    }
}
