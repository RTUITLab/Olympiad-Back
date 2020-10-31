using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class GroupTokenAndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteToken",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LessonsTime",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteToken",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LessonsTime",
                table: "Groups");
        }
    }
}
