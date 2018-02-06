using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApp.Migrations
{
    public partial class FirstLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestData_Exercises_ExerciseId",
                table: "TestData");

            migrationBuilder.DropIndex(
                name: "IX_TestData_ExerciseId",
                table: "TestData");
        }
    }
}
