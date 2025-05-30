namespace BibliotecasAPI.DAL.DTOs.ComentarioDTOs
{
    public class ComentarioConLibroDTO : ComentarioDTO
    {
        public int LibroId { get; set; }
        public required string LibroTitulo { get; set; }
    }
}
