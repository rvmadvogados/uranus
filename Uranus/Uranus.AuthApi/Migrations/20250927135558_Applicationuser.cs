using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uranus.AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class Applicationuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LegacyMd5Hash",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegacyMd5Hash",
                table: "AspNetUsers");
        }
    }
}
