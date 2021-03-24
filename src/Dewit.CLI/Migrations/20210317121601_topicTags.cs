using Microsoft.EntityFrameworkCore.Migrations;

namespace Dewit.CLI.Migrations
{
    public partial class topicTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Tasks",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Tasks");
        }
    }
}
