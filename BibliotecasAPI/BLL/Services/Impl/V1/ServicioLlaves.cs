using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioLlaves : IServicioLlaves
    {
        private readonly IRepositorioLlaves _repositorioLlaves;

        public ServicioLlaves(IRepositorioLlaves repositorioLlaves)
        {
            _repositorioLlaves = repositorioLlaves;
        }
        
        public async Task<IEnumerable<LlaveDTO>> ObtenerLlaves()
        {
            return await _repositorioLlaves.ObtenerLlaves();
        }
        public async Task<ActionResult<LlaveDTO>> ObtenerLlavePorId(int id)
        {
            return await _repositorioLlaves.ObtenerLlavePorId(id);
        }
        public async Task<ActionResult> AnadirLlave(LlaveCreacionDTO llave, string nombreEndpoint)
        {
            return await _repositorioLlaves.AnadirLlave(llave, nombreEndpoint);
        }

        public async Task<ActionResult> ActualizarLlave(int id, LlaveActualizacionDTO llave)
        {
            return await _repositorioLlaves.ActualizarLlave(id, llave);
        }

        public async Task<ActionResult> BorrarLlave(int id)
        {
            return await _repositorioLlaves.BorrarLlave(id);
        }

    }
}
