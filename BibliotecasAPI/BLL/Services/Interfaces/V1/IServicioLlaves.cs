using BibliotecasAPI.DAL.Model.Entidades;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioLlaves
    {
        Task<LlaveAPI> CrearLlave(string usuarioId, TipoLlave tipoLlave);
        string GenerarLlave();
    }
}
