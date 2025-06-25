using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V2
{
    [Route("api/v2/llaves")]
    [Authorize]
    [ApiController]
    [DeshabilitarLimitePeticiones]
    public class LlavesController : ControllerBase
    {
        private readonly IServicioLlaves _servicioLlaves;

        public LlavesController(IServicioLlaves servicioLlaves)
        {
            _servicioLlaves = servicioLlaves;
        }

        [HttpGet]
        public async Task<IEnumerable<LlaveDTO>> Get()
        {
            return await _servicioLlaves.ObtenerLlaves();
        }

        [HttpGet("{id:int}", Name = "ObtenerLlaveV2")]
        public async Task<ActionResult<LlaveDTO>> Get(int id)
        {
            return await _servicioLlaves.ObtenerLlavePorId(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LlaveCreacionDTO llave)
        {
            return await _servicioLlaves.AnadirLlave(llave, "ObtenerLlaveV2");
        }

        [HttpPut("{id:int}", Name = "ActualizarLlaveV2")]
        public async Task<ActionResult> Put(int id, LlaveActualizacionDTO llave)
        {
            return await _servicioLlaves.ActualizarLlave(id, llave);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioLlaves.BorrarLlave(id);
        }
    }
}
