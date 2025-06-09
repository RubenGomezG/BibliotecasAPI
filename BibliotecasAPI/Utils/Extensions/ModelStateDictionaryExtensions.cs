using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BibliotecasAPI.Utils.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static BadRequestObjectResult ContruirProblemDetail(this ModelStateDictionary modelState)
        {
            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        }

        public static BadRequestObjectResult RetornarLoginIncorrecto(this ModelStateDictionary modelState)
        {
            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Title = "Login Incorrecto.",
                Status = StatusCodes.Status400BadRequest
            };
            return new BadRequestObjectResult(problemDetails);
        }
    }
}
