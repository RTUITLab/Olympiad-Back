﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class CommentsRowNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowNumber",
                table: "Comments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowNumber",
                table: "Comments");
        }
    }
}
