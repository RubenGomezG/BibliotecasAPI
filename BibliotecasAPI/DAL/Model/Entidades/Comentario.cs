using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.Model.Entidades
{
    public class Comentario
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }
        public required string UsuarioId { get; set; }
        public bool Eliminado { get; set; }
        public Usuario? Usuario { get; set; }
        
    }
}
