using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;

namespace BibliotecasAPI.BLL.IServices
{
    public interface IServicioHash
    {
        ResultadoHashDTO Hash(string input);
        ResultadoHashDTO Hash(string input, byte[] sal);
    }
}
