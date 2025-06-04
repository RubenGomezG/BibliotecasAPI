namespace BibliotecasAPI.DAL.Model.Entidades
{
    public class Error
    {
        public Guid Id { get; set; }
        public required string Mensaje { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Fecha { get; set; }
    }
}
