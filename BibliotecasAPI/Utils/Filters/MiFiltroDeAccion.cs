using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecasAPI.Utils.Filters
{
    public class MiFiltroDeAccion : IActionFilter
    {
        private readonly ILogger<MiFiltroDeAccion> _logger;

        public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger)
        {
            _logger = logger;
        }
        // Antes de la acción
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Ejecutando la acción");
        }

        // Después de la acción
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Acción ejecutada");
        }

        
    }
}
