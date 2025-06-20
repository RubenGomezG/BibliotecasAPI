using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecasAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreaStoredProcedure_Facturas_Crear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(
			@"CREATE PROCEDURE Facturas_Crear
				-- Add the parameters for the stored procedure here
				@fechaInicio DATETIME,
				@fechaFin DATETIME
			AS
			BEGIN
				-- SET NOCOUNT ON added to prevent extra result sets from
				-- interfering with SELECT statements.
				SET NOCOUNT ON;

				-- Insert statements for procedure here

				DECLARE @importePorPeticion decimal(4,4) = 1.0/2

				INSERT INTO Facturas(UsuarioId, Importe, FechaEmision, FechaLimitePago, Pagada)
				SELECT UsuarioId, COUNT(*) * @importePorPeticion as Importe,
				GETDATE() AS FechaEmision, DATEADD(d, 60, GETDATE()) as FechaLimitePago,
				0 as Pagada
				FROM Peticiones 
				INNER JOIN LlavesAPI 
				ON LlavesAPI.Id = Peticiones.LlaveId
				WHERE FechaPeticion >= @fechaInicio AND FechaPeticion < @fechaFin
				GROUP BY UsuarioId

				INSERT INTO FacturasEmitidas(Mes, Año)
				SELECT
					CASE MONTH(GETDATE())
					WHEN 1 then 12
					ELSE MONTH(GETDATE()) - 1 END AS MES,

					CASE MONTH(GETDATE())
					WHEN 1 then YEAR(GETDATE()) - 1
					ELSE YEAR(GETDATE()) END AS AÑO
			END"
			);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE Facturas_Crear");
        }
    }
}
