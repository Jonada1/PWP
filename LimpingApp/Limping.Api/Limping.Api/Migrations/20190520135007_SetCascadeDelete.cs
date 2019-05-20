using Microsoft.EntityFrameworkCore.Migrations;

namespace Limping.Api.Migrations
{
    public partial class SetCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LimpingTests_AppUsers_AppUserId",
                table: "LimpingTests");

            migrationBuilder.AddForeignKey(
                name: "FK_LimpingTests_AppUsers_AppUserId",
                table: "LimpingTests",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LimpingTests_AppUsers_AppUserId",
                table: "LimpingTests");

            migrationBuilder.AddForeignKey(
                name: "FK_LimpingTests_AppUsers_AppUserId",
                table: "LimpingTests",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
