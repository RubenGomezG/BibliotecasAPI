using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BibliotecasAPI.BLL.Services.Interfaces.V1
{
    public interface IServicioUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
        string? ObtenerUsuarioId();
    }
}
