
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
                    Console.WriteLine("Ejecutando proceso de emisión de facturas");
                    await EmitirFacturas();
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                //Podemos ejecutar código cuando se detenga el job
            }
            
        }

        private async Task EmitirFacturas()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var hoy = DateTime.Today;
                var fechaComparacion = hoy.AddMonths(-1);

                var facturasYaEmitidas = await context.FacturasEmitidas
                    .AnyAsync(x => x.Año == fechaComparacion.Year && x.Mes == fechaComparacion.Month);

                if (!facturasYaEmitidas)
                {
                    if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
                    {
                        var fechaInicio = new DateTime(fechaComparacion.Year, fechaComparacion.Month, 1);
                        var fechaFin = fechaInicio.AddMonths(1);
                        await context.Database.ExecuteSqlAsync($"EXEC Facturas_Crear {fechaInicio.ToString("yyyy-MM-dd")}, {fechaFin.ToString("yyyy-MM-dd")}");
                    }
                }
            }
        }
    }
}
