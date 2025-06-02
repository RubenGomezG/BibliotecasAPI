namespace BibliotecasAPI.DAL.DTOs.UsuarioDTOs
{
    public class RespuestaAutenticacionDTO
    {
        public required string Token { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}
