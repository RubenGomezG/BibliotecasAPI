using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioRestriccionesDominio : IServicioRestriccionesDominio
    {
        private readonly IRepositorioRestriccionesDominio _repositorioRestriccionesDominio;

        public ServicioRestriccionesDominio(IRepositorioRestriccionesDominio repositorioRestriccionesDominio)
        {
            _repositorioRestriccionesDominio = repositorioRestriccionesDominio;
        }

        public async Task<ActionResult> AnadirRestriccionDominio(RestriccionDominioCreacionDTO restriccion)
        {
            return await _repositorioRestriccionesDominio.AnadirRestriccionDominio(restriccion);
        }

        public async Task<ActionResult> ActualizarRestriccionDominio(int id, RestriccionDominioActualizacionDTO restriccion)
        {
            return await _repositorioRestriccionesDominio.ActualizarRestriccionDominio(id, restriccion);
        }

        public async Task<ActionResult> BorrarRestriccionDominio(int id)
        {
            return await _repositorioRestriccionesDominio.BorrarRestriccionDominio(id);
        }
    }
}
