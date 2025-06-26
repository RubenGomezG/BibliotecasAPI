using BibliotecasAPI.DAL.Datos;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Jobs
{
    public class FacturasBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public FacturasBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (IServiceScope scope = _serviceProvider.CreateScope())
                    {
                        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        await EmitirFacturas(context);
                        await SetUsuarioTieneDeuda(context);
                        await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //Podemos ejecutar código cuando se detenga el job
            }
            
        }

        private async Task SetUsuarioTieneDeuda(ApplicationDbContext context)
        {
            await context.Database.ExecuteSqlAsync($"EXEC SetUsuarioTieneDeuda");
        }

        private async Task EmitirFacturas(ApplicationDbContext context)
        {
            DateTime hoy = DateTime.Today;
            DateTime fechaComparacion = hoy.AddMonths(-1);

            bool facturasYaEmitidas = await context.FacturasEmitidas
                .AnyAsync(x => x.Año == fechaComparacion.Year && x.Mes == fechaComparacion.Month);

            if (!facturasYaEmitidas)
            {
                DateTime fechaInicio = new DateTime(fechaComparacion.Year, fechaComparacion.Month, 1);
                DateTime fechaFin = fechaInicio.AddMonths(1);
                await context.Database.ExecuteSqlAsync($"EXEC Facturas_Crear {fechaInicio.ToString("yyyy-MM-dd")}, {fechaFin.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
