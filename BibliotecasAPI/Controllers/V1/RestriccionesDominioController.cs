using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/restriccionesDominio")]
    [Authorize]
    [DeshabilitarLimitePeticiones]
    public class RestriccionesDominioController : ControllerBase
    {
        private readonly IServicioRestriccionesDominio _servicioRestriccionesDominio;

        public RestriccionesDominioController(IServicioRestriccionesDominio servicioRestriccionesDominio)
        {
            _servicioRestriccionesDominio = servicioRestriccionesDominio;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionDominioCreacionDTO restriccion)
        {
            return await _servicioRestriccionesDominio.AnadirRestriccionDominio(restriccion);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestriccionDominioActualizacionDTO restriccion)
        {
            return await _servicioRestriccionesDominio.ActualizarRestriccionDominio(id, restriccion);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioRestriccionesDominio.BorrarRestriccionDominio(id);
        }
    }
}
