﻿using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs
{
    public class RestriccionDominioDTO
    {
        public int Id { get; set; }
        [Required]
        public required string Dominio { get; set; }
    }
}
