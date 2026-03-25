using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uranus.AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class AvailableClaimParentClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailableClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ParentClaimId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableClaims_AvailableClaims_ParentClaimId",
                        column: x => x.ParentClaimId,
                        principalTable: "AvailableClaims",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableClaims_ParentClaimId",
                table: "AvailableClaims",
                column: "ParentClaimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableClaims");
        }
    }
}
