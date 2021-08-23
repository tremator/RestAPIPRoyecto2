using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoWeb2.Migrations
{
    public partial class userTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tags",
                table: "news");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "tags",
                table: "news",
                type: "text[]",
                nullable: true);
        }
    }
}
