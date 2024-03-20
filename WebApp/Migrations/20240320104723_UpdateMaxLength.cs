using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OutData",
                table: "TestData",
                type: "character varying(1000000)",
                maxLength: 1000000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500000)",
                oldMaxLength: 500000);

            migrationBuilder.AlterColumn<string>(
                name: "InData",
                table: "TestData",
                type: "character varying(1000000)",
                maxLength: 1000000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500000)",
                oldMaxLength: 500000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OutData",
                table: "TestData",
                type: "character varying(500000)",
                maxLength: 500000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000000)",
                oldMaxLength: 1000000);

            migrationBuilder.AlterColumn<string>(
                name: "InData",
                table: "TestData",
                type: "character varying(500000)",
                maxLength: 500000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000000)",
                oldMaxLength: 1000000);
        }
    }
}
