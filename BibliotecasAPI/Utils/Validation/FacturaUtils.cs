using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Utils.Validation
{
    public static class FacturaUtils
    {
        public static ActionResult ValidarFactura(Factura? facturaDB, IHttpContextAccessor httpContextAccessor)
        {
            if (facturaDB == null)
            {
                return new NotFoundResult();
            }

            if (facturaDB.Pagada)
            {
                Controller? controller = httpContextAccessor.HttpContext!.GetEndpoint()!.Metadata.GetMetadata<Controller>();
                controller!.ModelState.AddModelError(nameof(facturaDB.Id), "La factura ya está pagada");
                return controller.ValidationProblem();
            }
            return new NoContentResult();
        }

        public static Task<bool> HayFacturasPendientesVencidas(ApplicationDbContext context, Factura? facturaDB)
        {
            return context.Facturas.AnyAsync(x => x.UsuarioId == facturaDB!.UsuarioId &&
                                            !x.Pagada && x.FechaLimitePago < DateTime.Today);
        }
    }
}
