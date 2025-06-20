using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecasAPI.Migrations
{
    /// <inheritdoc />
    public partial class AnadidasTablasRestricciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestriccionesDominio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LlaveAPIId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesDominio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesDominio_LlavesAPI_LlaveAPIId",
                        column: x => x.LlaveAPIId,
                        principalTable: "LlavesAPI",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesIp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LlaveAPIId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesIp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesIp_LlavesAPI_LlaveAPIId",
                        column: x => x.LlaveAPIId,
                        principalTable: "LlavesAPI",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominio_LlaveAPIId",
                table: "RestriccionesDominio",
                column: "LlaveAPIId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesIp_LlaveAPIId",
                table: "RestriccionesIp",
                column: "LlaveAPIId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestriccionesDominio");

            migrationBuilder.DropTable(
                name: "RestriccionesIp");
        }
    }
}
