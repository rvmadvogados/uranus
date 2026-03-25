using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uranus.AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoFactorSetupTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresTwoFactorSetup",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresTwoFactorSetup",
                table: "AspNetUsers");
        }
    }
}
