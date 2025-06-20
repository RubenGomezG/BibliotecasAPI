using BibliotecasAPI.DAL.Model.Entidades;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
        string? ObtenerUsuarioId();
    }
}
