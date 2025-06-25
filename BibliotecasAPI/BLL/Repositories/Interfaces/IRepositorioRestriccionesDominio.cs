using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioRestriccionesDominio
    {
        Task<ActionResult> ActualizarRestriccionDominio(int id, RestriccionDominioActualizacionDTO restriccion);
        Task<ActionResult> AnadirRestriccionDominio(RestriccionDominioCreacionDTO restriccion);
        Task<ActionResult> BorrarRestriccionDominio(int id);
    }
}