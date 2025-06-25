using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioRestriccionesDominio
    {
        Task<ActionResult> ActualizarRestriccionDominio(int id, RestriccionDominioActualizacionDTO restriccion);
        Task<ActionResult> AnadirRestriccionDominio(RestriccionDominioCreacionDTO restriccion);
        Task<ActionResult> BorrarRestriccionDominio(int id);
    }
}
