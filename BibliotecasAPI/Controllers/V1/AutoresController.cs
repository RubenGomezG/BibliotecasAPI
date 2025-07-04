﻿using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.Utils.Filters.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BibliotecasAPI.Controllers.V1
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v1/autores")]    
    public class AutoresController : ControllerBase
    {
        private readonly IServicioAutores _servicioAutoresV1;

        public AutoresController(IServicioAutores servicioAutoresV1)
        {
            _servicioAutoresV1 = servicioAutoresV1;
        }
        
        [HttpGet(Name = "ObtenerAutoresV1")] // api/autores
        [AllowAnonymous]
        [ServiceFilter<HateoasAutoresAttribute>()]
        public async Task<IEnumerable<AutorDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await _servicioAutoresV1.GetAutores(paginacionDTO);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutorPorIdV1")] // api/autores/id
        [EndpointSummary("Obtiene autor por id")]
        [EndpointDescription("Obtiene autor por id. Incluye sus libros. Si el autor no existe, se retorna 404")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter<HateoasAutorAttribute>()]
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<AutorConLibrosDTO>> Get(int id)
        {
            return await _servicioAutoresV1.GetAutorPorId(id);
        }

        [HttpGet("filtrar", Name = "FiltrarAutoresV1")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> Filtrar([FromQuery] AutorFiltroDTO autorFiltroDTO)
        {
            return await _servicioAutoresV1.Filtrar(autorFiltroDTO);           
        }             

        [HttpPost(Name = "CrearAutorV1")]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            return await _servicioAutoresV1.AnadirAutor(autorCreacionDTO);
        }

        [HttpPost("autor-con-foto", Name = "CrearAutorConFotoV1")]
        public async Task<ActionResult> PostConFoto([FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _servicioAutoresV1.AnadirAutorConFoto(autorCreacionDTO);
        }

        [HttpPut("{id:int}", Name = "ActualizarAutorV1")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, [FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _servicioAutoresV1.ActualizarAutor(id, autorCreacionDTO);
        }

        [HttpPatch("{id:int}", Name = "PatchAutorV1")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
        {
            return await _servicioAutoresV1.PatchAutor(id, patchDoc);
        }

        [HttpDelete("{id:int}", Name = "BorrarAutorV1")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioAutoresV1.BorrarAutor(id);
        }
    }
}
