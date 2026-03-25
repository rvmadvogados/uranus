using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uranus.AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLastTwoFactorConfirmedToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastTwoFactorConfirmed",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTwoFactorConfirmed",
                table: "AspNetUsers");
        }
    }
}
