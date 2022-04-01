using Microsoft.EntityFrameworkCore.Migrations;
using Models.Solutions;

namespace WebApp.Migrations
{
    public partial class SolutionDocumentsResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<SolutionDocuments>(
                name: "DocumentsResult",
                table: "Solutions",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentsResult",
                table: "Solutions");
        }
    }
}
