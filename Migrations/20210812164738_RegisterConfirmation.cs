using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoWeb2.Migrations
{
    public partial class RegisterConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "auth_code",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "register_confirmation",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auth_code",
                table: "users");

            migrationBuilder.DropColumn(
                name: "register_confirmation",
                table: "users");
        }
    }
}
