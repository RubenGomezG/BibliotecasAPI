using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Authorization;
using System;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class GeneradorEnlaces : IGeneradorEnlaces
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GeneradorEnlaces(LinkGenerator linkGenerator,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            AuthorizationResult esAdmin = await ComprobarAdmin();
            GenerarEnlaces(autorDTO, esAdmin.Succeeded);
        }

        public async Task<ColeccionDeRecursosDTO<AutorDTO>> GenerarEnlaces(List<AutorDTO> autores)
        {
            AuthorizationResult esAdmin = await ComprobarAdmin();
            foreach (var dto in autores)
            {
                GenerarEnlaces(dto, esAdmin.Succeeded );
            }

            var resultado = new ColeccionDeRecursosDTO<AutorDTO> { Valores = autores };
            resultado.Enlaces.Add(new DatosHateoasDTO(
                Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "ObtenerAutoresV1", new { })!,
                Descripcion: "self",
                MetodoHTTP: "GET"
                ));

            if (esAdmin.Succeeded)
            {
                resultado.Enlaces.Add(new DatosHateoasDTO(
                Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "CrearAutorV1", new { })!,
                Descripcion: "autor-crear",
                MetodoHTTP: "POST"
                ));

                resultado.Enlaces.Add(new DatosHateoasDTO(
                    Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "CrearAutorConFotoV1", new { })!,
                    Descripcion: "autor-crear-con-foto",
                    MetodoHTTP: "POST"
                    ));
            }

            return resultado;
        }

        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {
            autorDTO.Enlaces.Add(new DatosHateoasDTO(
                Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "ObtenerAutorPorIdV1", new { id = autorDTO.Id })!,
                Descripcion: "self",
                MetodoHTTP: "GET"
            ));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatosHateoasDTO(
                 Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "ActualizarAutorV1", new { id = autorDTO.Id })!,
                 Descripcion: "autor-actualizar",
                 MetodoHTTP: "PUT"
             ));
                autorDTO.Enlaces.Add(new DatosHateoasDTO(
                    Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "PatchAutorV1", new { id = autorDTO.Id })!,
                    Descripcion: "autor-patch",
                    MetodoHTTP: "PATCH"
                ));
                autorDTO.Enlaces.Add(new DatosHateoasDTO(
                    Link: _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext!, "BorrarAutorV1", new { id = autorDTO.Id })!,
                    Descripcion: "autor-borrar",
                    MetodoHTTP: "DELETE"
                ));
            }
        }

        private async Task<AuthorizationResult> ComprobarAdmin()
        {
            var usuario = _httpContextAccessor.HttpContext!.User;
            return await _authorizationService.AuthorizeAsync(usuario, "esAdmin");
        }
    }
}
