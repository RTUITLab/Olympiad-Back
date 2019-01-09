using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class sulotionTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Solutions",
                newName: "SendingTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedTime",
                table: "Solutions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartCheckingTime",
                table: "Solutions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedTime",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "StartCheckingTime",
                table: "Solutions");

            migrationBuilder.RenameColumn(
                name: "SendingTime",
                table: "Solutions",
                newName: "Time");
        }
    }
}
