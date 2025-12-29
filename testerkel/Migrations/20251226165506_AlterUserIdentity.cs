using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testerkel.Migrations
{
    /// <inheritdoc />
    public partial class AlterUserIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableNotifications",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableNotifications",
                table: "AspNetUsers");
        }
    }
}
