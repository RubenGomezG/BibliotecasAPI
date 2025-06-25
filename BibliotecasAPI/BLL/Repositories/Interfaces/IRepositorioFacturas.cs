using BibliotecasAPI.DAL.DTOs.FacturaDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioFacturas
    {
        Task<ActionResult> Pagar(FacturaPagarDTO factura);
    }
}