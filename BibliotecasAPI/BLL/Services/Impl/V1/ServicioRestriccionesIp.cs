using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioRestriccionesIp : IServicioRestriccionesIp
    {
        private readonly IRepositorioRestriccionesIp _repositorioRestriccionesIp;

        public ServicioRestriccionesIp(IRepositorioRestriccionesIp repositorioRestriccionesIp)
        {
            _repositorioRestriccionesIp = repositorioRestriccionesIp;
        }

        public async Task<ActionResult> AnadirRestriccionIp(RestriccionIpCreacionDTO restriccion)
        {
            return await _repositorioRestriccionesIp.AnadirRestriccionIp(restriccion);
        }

        public async Task<ActionResult> ActualizarRestriccionIp(int id, RestriccionIpActualizacionDTO restriccion)
        {
            return await _repositorioRestriccionesIp.ActualizarRestriccionIp(id, restriccion);
        }

        public async Task<ActionResult> BorrarRestriccionIp(int id)
        {
            return await _repositorioRestriccionesIp.BorrarRestriccionIp(id);
        }
    }
}
