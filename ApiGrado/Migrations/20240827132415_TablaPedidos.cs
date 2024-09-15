﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGrado.Migrations
{
    /// <inheritdoc />
    public partial class TablaPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarritoItems");

            migrationBuilder.DropTable(
                name: "CarritoCompras");

            migrationBuilder.CreateTable(
                name: "PedidosCompras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PrecioTotal = table.Column<double>(type: "float", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosCompras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidosItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccesorioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PedidosComprasId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosItems_Accesorios_AccesorioId",
                        column: x => x.AccesorioId,
                        principalTable: "Accesorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidosItems_PedidosCompras_PedidosComprasId",
                        column: x => x.PedidosComprasId,
                        principalTable: "PedidosCompras",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosCompras_UsuarioId",
                table: "PedidosCompras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosItems_AccesorioId",
                table: "PedidosItems",
                column: "AccesorioId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosItems_PedidosComprasId",
                table: "PedidosItems",
                column: "PedidosComprasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosItems");

            migrationBuilder.DropTable(
                name: "PedidosCompras");

            migrationBuilder.CreateTable(
                name: "CarritoCompras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    PrecioTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarritoCompras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarritoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccesorioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    CarritoComprasId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Accesorios_AccesorioId",
                        column: x => x.AccesorioId,
                        principalTable: "Accesorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarritoItems_CarritoCompras_CarritoComprasId",
                        column: x => x.CarritoComprasId,
                        principalTable: "CarritoCompras",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoCompras_UsuarioId",
                table: "CarritoCompras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_AccesorioId",
                table: "CarritoItems",
                column: "AccesorioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_CarritoComprasId",
                table: "CarritoItems",
                column: "CarritoComprasId");
        }
    }
}
