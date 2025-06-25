using AutoMapper;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.ClassUtils;
using BibliotecasAPI.Utils.Extensions;
using BibliotecasAPI.Utils.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioLlaves : IRepositorioLlaves
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RepositorioLlaves(ApplicationDbContext context,
            IMapper mapper,
            IServicioUsuarios servicioUsuarios,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _servicioUsuarios = servicioUsuarios;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<LlaveDTO>> ObtenerLlaves()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var llaves = await _context.LlavesAPI
                .Include(llave => llave.RestriccionesDominio)
                .Include(llave => llave.RestriccionesIp)
                .Where(llaves => llaves.UsuarioId == usuarioId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LlaveDTO>>(llaves);
        }

        public async Task<ActionResult<LlaveDTO>> ObtenerLlavePorId(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var llave = await _context.LlavesAPI.FirstOrDefaultAsync(llaves => llaves.Id == id);

            ActionResult result = LlaveApiValidation.ValidarLlaveAPI(_servicioUsuarios, llave);
            if (result.GetType() != typeof(NoContentResult))
            {
                return result;
            }

            return _mapper.Map<LlaveDTO>(llave);
        }

        public async Task<ActionResult> AnadirLlave(LlaveCreacionDTO llave, string nombreEndpoint)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (llave.TipoLlave == TipoLlave.Gratuita)
            {
                var tieneLlaveGratuita = await _context.LlavesAPI.AnyAsync(llave => llave.TipoLlave == TipoLlave.Gratuita);

                if (tieneLlaveGratuita)
                {
                    Controller? controller = _httpContextAccessor.HttpContext!.GetEndpoint()!.Metadata.GetMetadata<Controller>();
                    controller!.ModelState.ConstruirProblemDetail("El usuario ya tiene una llave gratuita");
                    return controller.ValidationProblem();
                }
            }

            var llaveAPI = await LlaveApiUtils.CrearLlave(_context, usuarioId!, llave.TipoLlave);
            var llaveDTO = _mapper.Map<LlaveDTO>(llaveAPI);
            return new CreatedAtRouteResult(nombreEndpoint, new { id = llaveAPI.Id }, llaveDTO);
        }

        public async Task<ActionResult> ActualizarLlave(int id, LlaveActualizacionDTO llave)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == id);

            ActionResult result = LlaveApiValidation.ValidarLlaveAPI(_servicioUsuarios, llaveDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                if (llave.ActualizarLlave)
                {
                    llaveDB!.Llave = LlaveApiUtils.GenerarLlave();
                    llaveDB.Activa = llave.Activa;
                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }

        public async Task<ActionResult> BorrarLlave(int id)
        {
            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == id);

            ActionResult result = LlaveApiValidation.ValidarLlaveAPI(_servicioUsuarios, llaveDB);
            if (result.GetType() == typeof(NoContentResult))
            {
                if (llaveDB!.TipoLlave == TipoLlave.Gratuita)
                {
                    Controller? controller = _httpContextAccessor.HttpContext!.GetEndpoint()!.Metadata.GetMetadata<Controller>();
                    controller!.ModelState.ConstruirProblemDetail("No puedes borrar una llave gratuita");
                    return controller.ValidationProblem();
                }
                _context.Remove(llaveDB);
                await _context.SaveChangesAsync();
            }

            return result;
        }
    }
}
