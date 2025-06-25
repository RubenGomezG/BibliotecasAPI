using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.FacturaDTOs;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V2
{
    [Route("api/v2/facturas")]
    [ApiController]
    [Authorize]
    [DeshabilitarLimitePeticiones]
    public class FacturasController : ControllerBase
    {
        private readonly IServicioFacturas _servicioFacturas;

        public FacturasController(IServicioFacturas servicioFacturas)
        {
            _servicioFacturas = servicioFacturas;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(FacturaPagarDTO factura)
        {
            return await _servicioFacturas.Pagar(factura);
        }
    }
}
