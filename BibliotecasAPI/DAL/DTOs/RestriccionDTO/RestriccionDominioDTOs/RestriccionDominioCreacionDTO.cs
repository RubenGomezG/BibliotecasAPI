﻿using System.ComponentModel.DataAnnotations;

namespace BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionDominioDTOs
{
    public class RestriccionDominioCreacionDTO
    {
        public int LlaveId { get; set; }
        [Required]
        public required string Dominio { get; set; }
    }
}
