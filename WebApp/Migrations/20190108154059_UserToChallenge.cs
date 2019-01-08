using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class UserToChallenge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChallengeAccessType",
                table: "Challenges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserToChallenges",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    UserId1 = table.Column<Guid>(nullable: true),
                    ChallengeId = table.Column<Guid>(nullable: false),
                    ChallengeId1 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToChallenges", x => new { x.ChallengeId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserToChallenges_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToChallenges_Challenges_ChallengeId1",
                        column: x => x.ChallengeId1,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToChallenges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToChallenges_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToChallenges_ChallengeId1",
                table: "UserToChallenges",
                column: "ChallengeId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserToChallenges_UserId",
                table: "UserToChallenges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToChallenges_UserId1",
                table: "UserToChallenges",
                column: "UserId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToChallenges");

            migrationBuilder.DropColumn(
                name: "ChallengeAccessType",
                table: "Challenges");
        }
    }
}
