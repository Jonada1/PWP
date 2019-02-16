using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Limping.Api.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LimpingTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TestData = table.Column<string>(nullable: true),
                    AppUserId = table.Column<string>(nullable: true),
                    TestAnalysisId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimpingTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimpingTests_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EndValue = table.Column<double>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LimpingSeverity = table.Column<int>(nullable: false),
                    LimpingTestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAnalyses_LimpingTests_LimpingTestId",
                        column: x => x.LimpingTestId,
                        principalTable: "LimpingTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LimpingTests_AppUserId",
                table: "LimpingTests",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAnalyses_LimpingTestId",
                table: "TestAnalyses",
                column: "LimpingTestId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestAnalyses");

            migrationBuilder.DropTable(
                name: "LimpingTests");

            migrationBuilder.DropTable(
                name: "AppUsers");
        }
    }
}
