﻿using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.Model.Entidades
{
    public class RestriccionDominio
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        [Required]
        public required string Dominio { get; set; }
        public LlaveAPI? Llave { get; set; }
    }
}
