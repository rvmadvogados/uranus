using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uranus.AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLastFailedLoginDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastFailedLoginDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastFailedLoginDate",
                table: "AspNetUsers");
        }
    }
}
