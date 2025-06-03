using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecasAPI.Migrations
{
    /// <inheritdoc />
    public partial class ComentariosBorradoLogico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Comentarios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Comentarios");
        }
    }
}
