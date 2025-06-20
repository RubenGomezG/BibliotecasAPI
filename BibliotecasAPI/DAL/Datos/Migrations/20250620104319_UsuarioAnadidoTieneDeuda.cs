using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecasAPI.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioAnadidoTieneDeuda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TieneDeuda",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TieneDeuda",
                table: "AspNetUsers");
        }
    }
}
