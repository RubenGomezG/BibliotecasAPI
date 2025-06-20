using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.Model.Entidades
{
    public class RestriccionIp
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        [Required]
        public required string Ip { get; set; }
        public LlaveAPI? Llave { get; set; }
    }
}
