using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Limping.Api.Migrations
{
    public partial class RemovedSomeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LimpingTestTestComparisons");

            migrationBuilder.DropTable(
                name: "TestComparisons");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestComparisons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ComparisonDate = table.Column<DateTime>(nullable: false),
                    ComparisonResults = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestComparisons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LimpingTestTestComparisons",
                columns: table => new
                {
                    LimpingTestId = table.Column<Guid>(nullable: false),
                    TestComparisonId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimpingTestTestComparisons", x => new { x.LimpingTestId, x.TestComparisonId });
                    table.ForeignKey(
                        name: "FK_LimpingTestTestComparisons_LimpingTests_LimpingTestId",
                        column: x => x.LimpingTestId,
                        principalTable: "LimpingTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LimpingTestTestComparisons_TestComparisons_TestComparisonId",
                        column: x => x.TestComparisonId,
                        principalTable: "TestComparisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LimpingTestTestComparisons_TestComparisonId",
                table: "LimpingTestTestComparisons",
                column: "TestComparisonId");
        }
    }
}
