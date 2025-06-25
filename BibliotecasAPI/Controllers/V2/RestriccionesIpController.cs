using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V2
{
    [ApiController]
    [Route("api/v2/restriccionesIp")]
    [Authorize]
    [DeshabilitarLimitePeticiones]
    public class RestriccionesIpController : ControllerBase
    {
        private readonly IServicioRestriccionesIp _servicioRestriccionesIp;

        public RestriccionesIpController(IServicioRestriccionesIp servicioRestriccionesIp)
        {
            _servicioRestriccionesIp = servicioRestriccionesIp;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionIpCreacionDTO restriccion)
        {
            return await _servicioRestriccionesIp.AnadirRestriccionIp(restriccion);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestriccionIpActualizacionDTO restriccion)
        {
            return await _servicioRestriccionesIp.ActualizarRestriccionIp(id, restriccion);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioRestriccionesIp.BorrarRestriccionIp(id);
        }
    }
}
