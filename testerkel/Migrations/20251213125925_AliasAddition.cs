using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testerkel.Migrations
{
    /// <inheritdoc />
    public partial class AliasAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitAliases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitType = table.Column<byte>(type: "tinyint", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitAliases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitAliases_Alias",
                table: "UnitAliases",
                column: "Alias",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitAliases");
        }
    }
}
