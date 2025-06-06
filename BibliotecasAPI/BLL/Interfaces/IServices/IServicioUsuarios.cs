using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BibliotecasAPI.BLL.Interfaces.IServices
{
    public interface IServicioUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
    }
}
