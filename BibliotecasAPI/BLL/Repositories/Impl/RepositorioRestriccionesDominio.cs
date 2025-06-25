using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioRestriccionesDominio : IRepositorioRestriccionesDominio
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;

        public RepositorioRestriccionesDominio(ApplicationDbContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        public async Task<ActionResult> AnadirRestriccionDominio(RestriccionDominioCreacionDTO restriccion)
        {
            LlaveAPI? llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == restriccion.LlaveId);

            ActionResult result = LlaveApiValidation.ValidarLlaveAPI(_servicioUsuarios, llaveDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                RestriccionDominio restriccionDominio = new RestriccionDominio
                {
                    LlaveId = restriccion.LlaveId,
                    Dominio = restriccion.Dominio
                };

                _context.Add(restriccionDominio);
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<ActionResult> ActualizarRestriccionDominio(int id, RestriccionDominioActualizacionDTO restriccion)
        {
            RestriccionDominio? restriccionDB = await _context.RestriccionesDominio
                .Include(r => r.Llave)
                .FirstOrDefaultAsync(r => r.Id == id);

            ActionResult result = RestriccionesUtils.ValidarRestriccionDominio(_servicioUsuarios, restriccionDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                restriccionDB!.Dominio = restriccion.Dominio;
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<ActionResult> BorrarRestriccionDominio(int id)
        {
            RestriccionDominio? restriccionDB = await _context.RestriccionesDominio
                .Include(r => r.Llave)
                .FirstOrDefaultAsync(r => r.Id == id);

            ActionResult result = RestriccionesUtils.ValidarRestriccionDominio(_servicioUsuarios, restriccionDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                _context.Remove(restriccionDB!);
                await _context.SaveChangesAsync();
            }

            return result;
        }
    }
}
