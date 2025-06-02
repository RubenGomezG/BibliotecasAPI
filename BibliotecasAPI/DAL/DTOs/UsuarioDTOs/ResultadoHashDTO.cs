namespace BibliotecasAPI.DAL.DTOs.UsuarioDTOs
{
    public class ResultadoHashDTO
    {
        public required string Hash { get; set; }
        public required byte[] Sal { get; set; }
    }
}
