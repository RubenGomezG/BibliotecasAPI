using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioRestriccionesIp : IRepositorioRestriccionesIp
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;

        public RepositorioRestriccionesIp(ApplicationDbContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        public async Task<ActionResult> AnadirRestriccionIp(RestriccionIpCreacionDTO restriccion)
        {
            LlaveAPI? llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == restriccion.LlaveId);

            ActionResult result = LlaveApiUtils.ValidarLlaveAPI(_servicioUsuarios, llaveDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                RestriccionIp restriccionIp = new RestriccionIp
                {
                    LlaveId = restriccion.LlaveId,
                    Ip = restriccion.Ip
                };

                _context.Add(restriccionIp);
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<ActionResult> ActualizarRestriccionIp(int id, RestriccionIpActualizacionDTO restriccion)
        {
            RestriccionIp? restriccionDB = await _context.RestriccionesIp
                .Include(r => r.Llave)
                .FirstOrDefaultAsync(r => r.Id == id);

            ActionResult result = RestriccionesUtils.ValidarRestriccionIp(_servicioUsuarios, restriccionDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                restriccionDB!.Ip = restriccion.Ip;
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<ActionResult> BorrarRestriccionIp(int id)
        {
            RestriccionIp? restriccionDB = await _context.RestriccionesIp
                .Include(r => r.Llave)
                .FirstOrDefaultAsync(r => r.Id == id);

            ActionResult result = RestriccionesUtils.ValidarRestriccionIp(_servicioUsuarios, restriccionDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                _context.Remove(restriccionDB!);
                await _context.SaveChangesAsync();
            }

            return result;
        }
    }
}
