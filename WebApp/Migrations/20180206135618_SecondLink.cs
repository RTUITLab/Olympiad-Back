using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApp.Migrations
{
    public partial class SecondLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Solutions_ExerciseId",
                table: "Solutions",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_UserId",
                table: "Solutions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Exercises_ExerciseId",
                table: "Solutions",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "ExerciseID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_AspNetUsers_UserId",
                table: "Solutions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Exercises_ExerciseId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_AspNetUsers_UserId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_ExerciseId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_UserId",
                table: "Solutions");
        }
    }
}
