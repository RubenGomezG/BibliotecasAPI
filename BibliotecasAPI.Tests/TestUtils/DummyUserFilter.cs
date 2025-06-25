using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BibliotecasAPI.Tests.TestUtils
{
    public class DummyUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //antes de la acción
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("email", "ejemplo@hotmail.com")
            }, "prueba"));

            await next();

            //después de la acción
        }
    }
}
