using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioLlaves
    {
        Task<ActionResult> ActualizarLlave(int id, LlaveActualizacionDTO llave);
        Task<ActionResult> AnadirLlave(LlaveCreacionDTO llave, string nombreEndpoint);
        Task<ActionResult> BorrarLlave(int id);
        Task<ActionResult<LlaveDTO>> ObtenerLlavePorId(int id);
        Task<IEnumerable<LlaveDTO>> ObtenerLlaves();
    }
}