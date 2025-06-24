using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.FacturaDTOs;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Controllers.V2
{
    [Route("api/v2/facturas")]
    [ApiController]
    [Authorize]
    [DeshabilitarLimitePeticiones]
    public class FacturasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(FacturaPagarDTO factura)
        {
            var facturaDB = await _context.Facturas
                .Include(factura => factura.Usuario)
                .FirstOrDefaultAsync(x => x.Id == factura.FacturaId);

            if (facturaDB == null) 
            { 
                return NotFound();
            }

            if (facturaDB.Pagada)
            {
                ModelState.AddModelError(nameof(factura.FacturaId), "La factura ya está pagada");
                return ValidationProblem();
            }

            facturaDB.Pagada = true;
            await _context.SaveChangesAsync();

            var hayFacturasPendientesVencidas = await _context.Facturas
                .AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId &&
                         !x.Pagada && x.FechaLimitePago < DateTime.Today);

            if (!hayFacturasPendientesVencidas)
            {
                facturaDB.Usuario!.TieneDeuda = false;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
