using BibliotecasAPI.DAL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V2
{
    [ApiController]
    [Route("api/v2")]
    [Authorize]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRootV1")]
        [AllowAnonymous]
        public async Task<IEnumerable<DatosHateoasDTO>> Get()
        {
            AuthorizationResult esAdmin = await _authorizationService.AuthorizeAsync(User, "esAdmin");
            List<DatosHateoasDTO> datosHateoas =
            [
                new DatosHateoasDTO(Link: Url.Link("ObtenerRootV2", new { })!, Descripcion: "self", MetodoHTTP: "GET"),
                new DatosHateoasDTO(Link: Url.Link("ObtenerAutoresV2", new { })!, Descripcion: "autores-obtener", MetodoHTTP: "GET"),
                new DatosHateoasDTO(Link: Url.Link("RegistrarV2", new { })!, Descripcion: "usuario-registrar", MetodoHTTP: "POST"),
                new DatosHateoasDTO(Link: Url.Link("LoginV2", new { })!, Descripcion: "usuario-login", MetodoHTTP: "POST"),
            ];
            

            if (esAdmin.Succeeded)
            {
                //Acciones para usuarios Administradores
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("CrearAutorV2", new { })!, Descripcion: "autor-crear", MetodoHTTP: "POST"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("CrearAutoresV2", new { })!, Descripcion: "autores-crear", MetodoHTTP: "POST"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("CrearLibroV2", new { })!, Descripcion: "libro-crear", MetodoHTTP: "POST"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("ObtenerUsuariosV2", new { })!, Descripcion: "usuarios-obtener", MetodoHTTP: "GET"));
            }

            if (User.Identity!.IsAuthenticated)
            {
                //Acciones para usuarios logeados
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("ActualizarUsuarioV2", new { })!, Descripcion: "usuario-actualizar", MetodoHTTP: "PUT"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("RenovarTokenV2", new { })!, Descripcion: "token-renovar", MetodoHTTP: "GET"));
            }
            return datosHateoas;
        }
    }
}
