using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class challenge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChallengeId",
                table: "Exercises",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ChallengeId",
                table: "Exercises",
                column: "ChallengeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Challenges_ChallengeId",
                table: "Exercises",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Challenges_ChallengeId",
                table: "Exercises");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_ChallengeId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "ChallengeId",
                table: "Exercises");
        }
    }
}
