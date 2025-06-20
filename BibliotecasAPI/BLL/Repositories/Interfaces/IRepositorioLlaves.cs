using BibliotecasAPI.DAL.Model.Entidades;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioLlaves
    {
        Task<LlaveAPI> CrearLlave(string usuarioId, TipoLlave tipoLlave);
        string GenerarLlave();
    }
}