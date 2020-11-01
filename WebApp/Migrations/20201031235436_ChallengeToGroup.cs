using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class ChallengeToGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToChallenge");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Challenges",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_GroupId",
                table: "Challenges",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Groups_GroupId",
                table: "Challenges",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Groups_GroupId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_GroupId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Challenges");

            migrationBuilder.CreateTable(
                name: "UserToChallenge",
                columns: table => new
                {
                    ChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToChallenge", x => new { x.ChallengeId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserToChallenge_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToChallenge_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToChallenge_UserId",
                table: "UserToChallenge",
                column: "UserId");
        }
    }
}
