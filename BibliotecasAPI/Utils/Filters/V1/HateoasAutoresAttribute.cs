using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecasAPI.Utils.Filters.V1
{
    public class HateoasAutoresAttribute : FiltroHateoasAttribute
    {
        private readonly IGeneradorEnlaces _generadorEnlaces;

        public HateoasAutoresAttribute(IGeneradorEnlaces generadorEnlaces)
        {
            _generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync
            (ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool incluirHateoas = DebeIncluirHateoas(context);

            if (!incluirHateoas)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            var modelo = result!.Value as List<AutorDTO> ?? throw new ArgumentNullException("Se esperaba una lista de Autor");

            context.Result = new OkObjectResult(await _generadorEnlaces.GenerarEnlaces(modelo));
            
            await next();
        }
    }
}
