using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Diagnostics;

namespace BibliotecasAPI.Utils.Middlewares
{
    public class ErrorHandler
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error!;

            var error = new Error()
            {
                Mensaje = exception.Message,
                StackTrace = exception.StackTrace,
                Fecha = DateTime.UtcNow
            };

            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
            dbContext.Add(error);
            await dbContext.SaveChangesAsync();
            await Results.InternalServerError(new
            {
                tipo = "error",
                mensaje = "Ha ocurrido un error inesperado",
                status = 500
            }).ExecuteAsync(context);
        }

    }
    public static class ErrorHandlerExtensions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseExceptionHandler(exceptionhandlerApp => exceptionhandlerApp.Run(async context => 
            {
                await ErrorHandler.InvokeAsync(context);
            }));
        }
    }
}

