using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class ExersiceTestDataLimits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TestDataGroups",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OutData",
                table: "TestData",
                type: "character varying(500000)",
                maxLength: 500000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InData",
                table: "TestData",
                type: "character varying(500000)",
                maxLength: 500000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestDataGroups_Title_ExerciseId",
                table: "TestDataGroups",
                columns: new[] { "Title", "ExerciseId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TestDataGroups_Title_ExerciseId",
                table: "TestDataGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TestDataGroups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "OutData",
                table: "TestData",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500000)",
                oldMaxLength: 500000);

            migrationBuilder.AlterColumn<string>(
                name: "InData",
                table: "TestData",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500000)",
                oldMaxLength: 500000);
        }
    }
}
