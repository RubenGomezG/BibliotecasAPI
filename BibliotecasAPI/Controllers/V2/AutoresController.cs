﻿using BibliotecasAPI.BLL.Services.Interfaces.V2;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.Utils.Filters.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BibliotecasAPI.Controllers.V2
{
    [Authorize(Policy = "esAdmin")]
    [ApiController]
    [Route("api/v2/autores")]    
    public class AutoresController : ControllerBase
    {
        private readonly IServicioAutoresV2 _servicioAutoresV2;

        public AutoresController(IServicioAutoresV2 servicioAutoresV2)
        {
            _servicioAutoresV2 = servicioAutoresV2;
        }

        [HttpGet(Name = "ObtenerAutoresV2")] // api/autores
        [AllowAnonymous]
        [ServiceFilter<HateoasAutoresAttribute>()]
        public async Task<IEnumerable<AutorDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await _servicioAutoresV2.GetAutores(paginacionDTO);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutorV2")] // api/autores/id
        [EndpointSummary("Obtiene autor por id")]
        [EndpointDescription("Obtiene autor por id. Incluye sus libros. Si el autor no existe, se retorna 404")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [OutputCache]
        public async Task<ActionResult<AutorConLibrosDTO>> Get(int id, bool incluirLibros = false)
        {
            return await _servicioAutoresV2.GetAutorPorIdV2(id, incluirLibros);
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult> Filtrar([FromQuery] AutorFiltroDTO autorFiltroDTO)
        {
            var autores = await _servicioAutoresV2.Filtrar(autorFiltroDTO);
            return Ok(autores);
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            return await _servicioAutoresV2.AnadirAutor(autorCreacionDTO);
        }

        [HttpPost("autor-con-foto")]
        public async Task<ActionResult> PostConFoto([FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _servicioAutoresV2.AnadirAutorConFoto(autorCreacionDTO);
        }

        [HttpPut("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Put(int id, [FromForm] AutorCreacionConFotoDTO autorCreacionDTO)
        {
            return await _servicioAutoresV2.ActualizarAutor(id, autorCreacionDTO);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
        {
            return await _servicioAutoresV2.PatchAutor(id, patchDoc);
        }

        [HttpDelete("{id:int}")] //Put/api/autores/{id}
        public async Task<ActionResult> Delete(int id)
        {
            return await _servicioAutoresV2.BorrarAutor(id);
        }
    }
}
