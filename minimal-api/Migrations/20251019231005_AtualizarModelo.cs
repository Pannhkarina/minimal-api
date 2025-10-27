using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Administradore",
                table: "Administradore");

            migrationBuilder.RenameTable(
                name: "Administradore",
                newName: "Administradores");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Administradores",
                table: "Administradores",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Administradores",
                table: "Administradores");

            migrationBuilder.RenameTable(
                name: "Administradores",
                newName: "Administradore");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Administradore",
                table: "Administradore",
                column: "Id");
        }
    }
}
