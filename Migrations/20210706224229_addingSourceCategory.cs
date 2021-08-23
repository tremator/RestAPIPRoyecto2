using Microsoft.EntityFrameworkCore.Migrations;

namespace ProyectoWeb2.Migrations
{
    public partial class addingSourceCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "category_id",
                table: "news_sources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_news_sources_category_id",
                table: "news_sources",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_news_sources_categorys_category_id",
                table: "news_sources",
                column: "category_id",
                principalTable: "categorys",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_news_sources_categorys_category_id",
                table: "news_sources");

            migrationBuilder.DropIndex(
                name: "ix_news_sources_category_id",
                table: "news_sources");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "news_sources");
        }
    }
}
