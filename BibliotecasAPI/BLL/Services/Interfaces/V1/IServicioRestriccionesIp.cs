using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioRestriccionesIp
    {
        Task<ActionResult> ActualizarRestriccionIp(int id, RestriccionIpActualizacionDTO restriccion);
        Task<ActionResult> AnadirRestriccionIp(RestriccionIpCreacionDTO restriccion);
        Task<ActionResult> BorrarRestriccionIp(int id);
    }
}
