using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class ExerciseDataGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestData_Exercises_ExerciseId",
                table: "TestData");

            migrationBuilder.DropIndex(
                name: "IX_TestData_ExerciseId",
                table: "TestData");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "TestData");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "TestData");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Exercises");

            migrationBuilder.AddColumn<Guid>(
                name: "ExerciseDataGroupId",
                table: "TestData",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TestDataGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    ExerciseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDataGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestDataGroups_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestData_ExerciseDataGroupId",
                table: "TestData",
                column: "ExerciseDataGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDataGroups_ExerciseId",
                table: "TestDataGroups",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestData_TestDataGroups_ExerciseDataGroupId",
                table: "TestData",
                column: "ExerciseDataGroupId",
                principalTable: "TestDataGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestData_TestDataGroups_ExerciseDataGroupId",
                table: "TestData");

            migrationBuilder.DropTable(
                name: "TestDataGroups");

            migrationBuilder.DropIndex(
                name: "IX_TestData_ExerciseDataGroupId",
                table: "TestData");

            migrationBuilder.DropColumn(
                name: "ExerciseDataGroupId",
                table: "TestData");

            migrationBuilder.AddColumn<Guid>(
                name: "ExerciseId",
                table: "TestData",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "TestData",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestData_ExerciseId",
                table: "TestData",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestData_Exercises_ExerciseId",
                table: "TestData",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "ExerciseID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
