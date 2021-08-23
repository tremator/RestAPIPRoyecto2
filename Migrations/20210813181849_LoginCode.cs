using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoWeb2.Migrations
{
    public partial class LoginCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_logged",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_logged",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "users");
        }
    }
}
