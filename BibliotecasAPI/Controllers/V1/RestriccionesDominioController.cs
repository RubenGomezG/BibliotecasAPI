using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionDominioDTOs;
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
        private readonly ApplicationDbContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;

        public RestriccionesDominioController(ApplicationDbContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpPost]
        public async Task<ActionResult> Post(RestriccionDominioCreacionDTO restriccion)
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

            var restriccionDominio = new RestriccionDominio
            {
                LlaveId = restriccion.LlaveId,
                Dominio = restriccion.Dominio
            };

            _context.Add(restriccionDominio);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RestriccionDominioActualizacionDTO restriccion)
        {
            var restriccionDB = await _context.RestriccionesDominio
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

            restriccionDB.Dominio = restriccion.Dominio;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await _context.RestriccionesDominio
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
