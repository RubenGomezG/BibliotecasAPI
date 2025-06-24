using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecasAPI.Migrations
{
    /// <inheritdoc />
    public partial class SP_UsuariosTienenDeuda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE SetUsuarioTieneDeuda
	            -- Add the parameters for the stored procedure here
            AS
            BEGIN
	            -- SET NOCOUNT ON added to prevent extra result sets from
	            -- interfering with SELECT statements.
	            SET NOCOUNT ON;

                UPDATE AspNetUsers
	            SET TieneDeuda = 'True'
	            FROM Facturas
	            INNER JOIN AspNetUsers
	            ON AspNetUsers.Id = Facturas.UsuarioId
	            WHERE Pagada = 'False' AND FechaLimitePago < GETDATE()
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE SetUsuarioTieneDeuda");
        }
    }
}
