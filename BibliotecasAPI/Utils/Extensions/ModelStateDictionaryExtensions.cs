using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BibliotecasAPI.Utils.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static BadRequestObjectResult ContruirProblemDetail(this ModelStateDictionary modelState, string error)
        {
            ValidationProblemDetails problemDetails = new ValidationProblemDetails(modelState)
            {
                Title = error,
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        }

        public static ActionResult RetornarLoginIncorrecto(this ModelStateDictionary modelState)
        {
            modelState.AddModelError(string.Empty, "Login incorrecto.");
            ValidationProblemDetails problemDetails = new ValidationProblemDetails(modelState)
            {
                Title = "Login Incorrecto.",
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        }
    }
}
