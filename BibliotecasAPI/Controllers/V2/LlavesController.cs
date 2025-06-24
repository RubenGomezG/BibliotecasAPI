using AutoMapper;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V2
{
    [Route("api/v2/llaves")]
    [Authorize]
    [ApiController]
    [DeshabilitarLimitePeticiones]
    public class LlavesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServicioLlaves _servicioLlaves;
        private readonly IServicioUsuarios _servicioUsuarios;

        public LlavesController(ApplicationDbContext context,
            IMapper mapper,
            IServicioLlaves servicioLlaves,
            IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _mapper = mapper;
            _servicioLlaves = servicioLlaves;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public async Task<IEnumerable<LlaveDTO>> Get()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var llaves = await _context.LlavesAPI
                .Include(llave => llave.RestriccionesDominio)
                .Include(llave => llave.RestriccionesIp)
                .Where(llaves => llaves.UsuarioId == usuarioId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LlaveDTO>>(llaves);
        }

        [HttpGet("{id:int}", Name = "ObtenerLlaveV2")]
        public async Task<ActionResult<LlaveDTO>> Get(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var llave = await _context.LlavesAPI.FirstOrDefaultAsync(llaves => llaves.Id == id);

            if (llave == null)
            {
                return NotFound();
            }

            if (llave.UsuarioId != usuarioId)
            {
                return Forbid();
            }
            return _mapper.Map<LlaveDTO>(llave);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LlaveCreacionDTO llave)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (llave.TipoLlave == TipoLlave.Gratuita)
            {
                var tieneLlaveGratuita = await _context.LlavesAPI.AnyAsync(llave => llave.TipoLlave == TipoLlave.Gratuita);

                if (tieneLlaveGratuita)
                {
                    ModelState.AddModelError(nameof(llave.TipoLlave), "El usuario ya tiene una llave gratuita");
                    return ValidationProblem();
                }
            }

            var llaveAPI = await _servicioLlaves.CrearLlave(usuarioId!, llave.TipoLlave);
            var llaveDTO = _mapper.Map<LlaveDTO>(llaveAPI);
            return CreatedAtRoute("ObtenerLlaveV2", new { id = llaveAPI.Id }, llaveDTO);
        }

        [HttpPut("{id:int}", Name = "ActualizarLlaveV2")]
        public async Task<ActionResult> Put(int id, LlaveActualizacionDTO llave)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == id);

            if (llaveDB == null)
            {
                return NotFound();
            }

            if (usuarioId != llaveDB.UsuarioId)
            {
                return Forbid();
            }

            if (llave.ActualizarLlave)
            {
                llaveDB.Llave = _servicioLlaves.GenerarLlave();
            }

            llaveDB.Activa = llave.Activa;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var llaveDB = await _context.LlavesAPI.FirstOrDefaultAsync(llave => llave.Id == id);

            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            if (usuarioId != llaveDB.UsuarioId)
            {
                return Forbid();
            }

            if (llaveDB.TipoLlave == TipoLlave.Gratuita)
            {
                ModelState.AddModelError("", "No puedes borrar una llave gratuita");
                return ValidationProblem();
            }


            _context.Remove(llaveDB);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
