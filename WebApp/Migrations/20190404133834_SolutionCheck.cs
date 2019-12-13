using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class SolutionCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolutionChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CheckedTime = table.Column<DateTime>(nullable: false),
                    ExampleIn = table.Column<string>(nullable: true),
                    ExampleOut = table.Column<string>(nullable: true),
                    ProgramOut = table.Column<string>(nullable: true),
                    ProgramErr = table.Column<string>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    SolutionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionChecks_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_SolutionId",
                table: "SolutionChecks",
                column: "SolutionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolutionChecks");
        }
    }
}
