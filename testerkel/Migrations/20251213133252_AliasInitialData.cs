using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace testerkel.Migrations
{
    /// <inheritdoc />
    public partial class AliasInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "UnitAliases",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.InsertData(
                table: "UnitAliases",
                columns: new[] { "Id", "Alias", "UnitType" },
                values: new object[,]
                {
                    { 1, "Ad", (byte)1 },
                    { 2, "Ad.", (byte)1 },
                    { 3, "Adet", (byte)1 },
                    { 4, "Ad/Tl", (byte)7 },
                    { 5, "Kg", (byte)2 },
                    { 6, "Kg.", (byte)2 },
                    { 7, "Kilogram", (byte)2 },
                    { 8, "Lt", (byte)3 },
                    { 9, "L", (byte)3 },
                    { 10, "Litre", (byte)3 },
                    { 11, "Kutu", (byte)4 },
                    { 12, "m", (byte)5 },
                    { 13, "m.", (byte)5 },
                    { 16, "Mt", (byte)5 },
                    { 17, "Mt.", (byte)5 },
                    { 18, "Metre", (byte)5 },
                    { 19, "m^2", (byte)9 },
                    { 20, "m2", (byte)9 },
                    { 23, "m3", (byte)10 },
                    { 26, "m^3", (byte)10 },
                    { 27, "Paket", (byte)11 },
                    { 28, "Takım", (byte)12 },
                    { 29, "Tk", (byte)12 },
                    { 30, "Tk.", (byte)12 },
                    { 31, "Ton", (byte)13 },
                    { 32, "T", (byte)13 },
                    { 33, "Top", (byte)14 },
                    { 34, "Saat", (byte)6 },
                    { 35, "Sa", (byte)6 },
                    { 36, "Sa.", (byte)6 },
                    { 37, "St", (byte)6 },
                    { 38, "Çift", (byte)8 },
                    { 39, "Cift", (byte)8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "UnitAliases",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "UnitAliases",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
