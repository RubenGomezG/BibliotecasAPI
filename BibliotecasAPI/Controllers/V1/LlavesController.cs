using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V1
{
    [Route("api/v1/llaves")]
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

        [HttpGet("{id:int}", Name = "ObtenerLlaveV1")]
        public async Task<ActionResult<LlaveDTO>> Get(int id)
        {
            return await _servicioLlaves.ObtenerLlavePorId(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LlaveCreacionDTO llave)
        {
            return await _servicioLlaves.AnadirLlave(llave, "ObtenerLlaveV1");
        }

        [HttpPut("{id:int}", Name = "ActualizarLlaveV1")]
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
