using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioRestriccionesIp
    {
        Task<ActionResult> ActualizarRestriccionIp(int id, RestriccionIpActualizacionDTO restriccion);
        Task<ActionResult> AnadirRestriccionIp(RestriccionIpCreacionDTO restriccion);
        Task<ActionResult> BorrarRestriccionIp(int id);
    }
}