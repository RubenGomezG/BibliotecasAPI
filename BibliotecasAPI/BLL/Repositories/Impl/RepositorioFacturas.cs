using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.FacturaDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioFacturas : IRepositorioFacturas
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RepositorioFacturas(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActionResult> Pagar(FacturaPagarDTO factura)
        {
            Factura? facturaDB = await _context.Facturas
                .Include(factura => factura.Usuario)
                .FirstOrDefaultAsync(x => x.Id == factura.FacturaId);

            ActionResult result = FacturaUtils.ValidarFactura(facturaDB, _httpContextAccessor);
            if (result.GetType() == typeof(NoContentResult))
            {
                facturaDB!.Pagada = true;
                await _context.SaveChangesAsync();

                bool hayFacturasPendientesVencidas = await FacturaUtils.HayFacturasPendientesVencidas(_context, facturaDB);

                if (!hayFacturasPendientesVencidas)
                {
                    facturaDB.Usuario!.TieneDeuda = false;
                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }
    }
}
