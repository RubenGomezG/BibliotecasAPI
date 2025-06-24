using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V2
{
    [ApiController]
    [Route("api/v2/restriccionesIp")]
    [Authorize]
    [DeshabilitarLimitePeticiones]
    public class RestriccionesIpController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;

        public RestriccionesIpController(ApplicationDbContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionIpCreacionDTO restriccion)
        {
            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == restriccion.LlaveId);

            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (llaveDB.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            var restriccionIp = new RestriccionIp
            {
                LlaveId = restriccion.LlaveId,
                Ip = restriccion.Ip
            };

            _context.Add(restriccionIp);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestriccionIpActualizacionDTO restriccion)
        {
            var restriccionDB = await _context.RestriccionesIp
                .Include(r => r.Llave)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (restriccionDB.Llave!.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            restriccionDB.Ip = restriccion.Ip;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await _context.RestriccionesIp
                .Include(r => r.Llave)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (restriccionDB.Llave!.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            _context.Remove(restriccionDB);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
