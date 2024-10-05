using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGrado.Migrations
{
    /// <inheritdoc />
    public partial class ColorBicicleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorBicicleta",
                table: "PedidosCompras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorBicicleta",
                table: "PedidosCompras");
        }
    }
}
