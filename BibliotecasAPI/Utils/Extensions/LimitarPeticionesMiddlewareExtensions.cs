using BibliotecasAPI.Utils.Middlewares;

namespace BibliotecasAPI.Utils.Extensions
{
    public static class LimitarPeticionesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }
}
