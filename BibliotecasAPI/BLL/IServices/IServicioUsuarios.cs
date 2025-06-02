using BibliotecasAPI.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BibliotecasAPI.BLL.IServices
{
    public interface IServicioUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
    }
}
