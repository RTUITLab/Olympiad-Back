using Microsoft.EntityFrameworkCore.Migrations;
using Models.Exercises;

namespace WebApp.Migrations
{
    public partial class ExerciseRestrictions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ExerciseRestrictions>(
                name: "Restrictions",
                table: "Exercises",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Restrictions",
                table: "Exercises");
        }
    }
}
