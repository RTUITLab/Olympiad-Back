using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebApp.Migrations
{
    public partial class buildlogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolutionBuildLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BuildedTime = table.Column<DateTime>(nullable: false),
                    Log = table.Column<string>(nullable: true),
                    SolutionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionBuildLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionBuildLogs_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolutionBuildLogs_SolutionId",
                table: "SolutionBuildLogs",
                column: "SolutionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolutionBuildLogs");
        }
    }
}
