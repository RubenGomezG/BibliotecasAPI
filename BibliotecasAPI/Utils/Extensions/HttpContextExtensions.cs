using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Utils.Extensions
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Append("cantidad-registros", cantidad.ToString());
        }
    }
}
