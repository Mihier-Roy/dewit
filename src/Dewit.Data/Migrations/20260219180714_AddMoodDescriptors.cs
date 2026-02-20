using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dewit.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoodDescriptors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoodDescriptors",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mood = table.Column<string>(type: "TEXT", nullable: false),
                    Descriptors = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodDescriptors", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MoodDescriptors");
        }
    }
}
