namespace BibliotecasAPI.DAL.DTOs.AutorDTOs
{
    public class AutorCreacionConFotoDTO : AutorCreacionDTO
    {
        public IFormFile? Foto { get; set; }
    }
}
