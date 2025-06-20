using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecasAPI.Migrations
{
    /// <inheritdoc />
    public partial class ArregladaRelaciónRestricciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesDominio_LlavesAPI_LlaveAPIId",
                table: "RestriccionesDominio");

            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesIp_LlavesAPI_LlaveAPIId",
                table: "RestriccionesIp");

            migrationBuilder.DropIndex(
                name: "IX_RestriccionesIp_LlaveAPIId",
                table: "RestriccionesIp");

            migrationBuilder.DropIndex(
                name: "IX_RestriccionesDominio_LlaveAPIId",
                table: "RestriccionesDominio");

            migrationBuilder.DropColumn(
                name: "LlaveAPIId",
                table: "RestriccionesIp");

            migrationBuilder.DropColumn(
                name: "LlaveAPIId",
                table: "RestriccionesDominio");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesIp_LlaveId",
                table: "RestriccionesIp",
                column: "LlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominio_LlaveId",
                table: "RestriccionesDominio",
                column: "LlaveId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesDominio_LlavesAPI_LlaveId",
                table: "RestriccionesDominio",
                column: "LlaveId",
                principalTable: "LlavesAPI",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesIp_LlavesAPI_LlaveId",
                table: "RestriccionesIp",
                column: "LlaveId",
                principalTable: "LlavesAPI",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesDominio_LlavesAPI_LlaveId",
                table: "RestriccionesDominio");

            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesIp_LlavesAPI_LlaveId",
                table: "RestriccionesIp");

            migrationBuilder.DropIndex(
                name: "IX_RestriccionesIp_LlaveId",
                table: "RestriccionesIp");

            migrationBuilder.DropIndex(
                name: "IX_RestriccionesDominio_LlaveId",
                table: "RestriccionesDominio");

            migrationBuilder.AddColumn<int>(
                name: "LlaveAPIId",
                table: "RestriccionesIp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LlaveAPIId",
                table: "RestriccionesDominio",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesIp_LlaveAPIId",
                table: "RestriccionesIp",
                column: "LlaveAPIId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominio_LlaveAPIId",
                table: "RestriccionesDominio",
                column: "LlaveAPIId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesDominio_LlavesAPI_LlaveAPIId",
                table: "RestriccionesDominio",
                column: "LlaveAPIId",
                principalTable: "LlavesAPI",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesIp_LlavesAPI_LlaveAPIId",
                table: "RestriccionesIp",
                column: "LlaveAPIId",
                principalTable: "LlavesAPI",
                principalColumn: "Id");
        }
    }
}
