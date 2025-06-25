using BibliotecasAPI.DAL.DTOs.FacturaDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioFacturas
    {
        Task<ActionResult> Pagar(FacturaPagarDTO factura);
    }
}
