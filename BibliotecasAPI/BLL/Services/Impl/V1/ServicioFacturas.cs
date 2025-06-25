using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.FacturaDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioFacturas : IServicioFacturas
    {
        private readonly IRepositorioFacturas _repositorioFacturas;

        public ServicioFacturas(IRepositorioFacturas repositorioFacturas)
        {
            _repositorioFacturas = repositorioFacturas;
        }

        public async Task<ActionResult> Pagar(FacturaPagarDTO factura)
        {
            return await _repositorioFacturas.Pagar(factura);
        }
    }
}
