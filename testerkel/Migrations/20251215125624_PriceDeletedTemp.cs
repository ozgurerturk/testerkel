using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testerkel.Migrations
{
    /// <inheritdoc />
    public partial class PriceDeletedTemp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPriceHistories");

            migrationBuilder.AddColumn<decimal>(
                name: "LineTotal",
                table: "StockTxns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "StockTxns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineTotal",
                table: "StockTxns");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "StockTxns");

            migrationBuilder.CreateTable(
                name: "ProductPriceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPriceHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistories_ProductId_EndDate",
                table: "ProductPriceHistories",
                columns: new[] { "ProductId", "EndDate" });
        }
    }
}
