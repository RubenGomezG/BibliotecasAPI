using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BibliotecasAPI.Utils.Filters
{
    public class FiltroTiempoEjecucion : IAsyncActionFilter
    {
        private readonly ILogger<FiltroTiempoEjecucion> _logger;

        public FiltroTiempoEjecucion(ILogger<FiltroTiempoEjecucion> logger)
        {
            this._logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"INICIO Acción: {context.ActionDescriptor.DisplayName}");

            await next();

            stopwatch.Stop();

            _logger.LogInformation($"FIN Acción: {context.ActionDescriptor.DisplayName} - Tiempo: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
