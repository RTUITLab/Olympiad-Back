using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class CheckToTestDataRecordLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TestDataId",
                table: "SolutionChecks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SolutionChecks_TestDataId",
                table: "SolutionChecks",
                column: "TestDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionChecks_TestData_TestDataId",
                table: "SolutionChecks",
                column: "TestDataId",
                principalTable: "TestData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolutionChecks_TestData_TestDataId",
                table: "SolutionChecks");

            migrationBuilder.DropIndex(
                name: "IX_SolutionChecks_TestDataId",
                table: "SolutionChecks");

            migrationBuilder.DropColumn(
                name: "TestDataId",
                table: "SolutionChecks");
        }
    }
}
