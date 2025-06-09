using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecasAPI.Utils.Filters
{
    public class FiltroHateoasAttribute : ResultFilterAttribute
    {
        protected bool DebeIncluirHateoas(ResultExecutingContext context)
        {
            if (context.Result is not ObjectResult result || !RespuestaExitosa(result))
            {
                return false;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("IncluirHateoas", out var header))
            {
                return false;
            }

            return string.Equals(header, "Y", StringComparison.OrdinalIgnoreCase);
        }
        private bool RespuestaExitosa(ObjectResult result)
        {
            if (result.Value == null)
            {
                return false;
            }

            if(result.StatusCode.HasValue && !result.StatusCode!.Value.ToString().StartsWith("2"))
            {
                return false;
            }

            return true;
        }
    }
}
