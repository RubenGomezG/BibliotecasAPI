using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.Model.Entidades;
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
        public async Task<LlaveAPI> CrearLlave(string usuarioId, TipoLlave tipoLlave)
        {
            return await _repositorioLlaves.CrearLlave(usuarioId, tipoLlave);
        }

        public string GenerarLlave()
        {
            return _repositorioLlaves.GenerarLlave();
        }
    }
}
