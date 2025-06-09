using BibliotecasAPI.DAL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRootV1")]
        [AllowAnonymous]
        public async Task<IEnumerable<DatosHateoasDTO>> Get()
        {
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            var datosHateoas = new List<DatosHateoasDTO>();
            datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("ObtenerRootV1", new { })!, Descripcion: "self", MetodoHTTP: "GET"));
            datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("ObtenerAutoresV1", new { })!, Descripcion: "autores-obtener", MetodoHTTP: "GET"));
            
            datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("RegistrarV1", new { })!, Descripcion: "usuario-registrar", MetodoHTTP: "POST"));
            datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("LoginV1", new { })!, Descripcion: "usuario-login", MetodoHTTP: "POST"));
            

            if (esAdmin.Succeeded)
            {
                //Acciones para usuarios Administradores
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("CrearAutorV1", new { })!, Descripcion: "autor-crear", MetodoHTTP: "POST"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("CrearAutoresV1", new { })!, Descripcion: "autores-crear", MetodoHTTP: "POST"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("CrearLibroV1", new { })!, Descripcion: "libro-crear", MetodoHTTP: "POST"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("ObtenerUsuariosV1", new { })!, Descripcion: "usuarios-obtener", MetodoHTTP: "GET"));
            }

            if (User.Identity!.IsAuthenticated)
            {
                //Acciones para usuarios logeados
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("ActualizarUsuarioV1", new { })!, Descripcion: "usuario-actualizar", MetodoHTTP: "PUT"));
                datosHateoas.Add(new DatosHateoasDTO(Link: Url.Link("RenovarTokenV1", new { })!, Descripcion: "token-renovar", MetodoHTTP: "GET"));
            }
            return datosHateoas;
        }
    }
}
