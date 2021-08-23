using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoWeb2.Migrations
{
    public partial class Tags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "tags",
                table: "news",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tags",
                table: "news");
        }
    }
}
