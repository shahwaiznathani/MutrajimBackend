using Microsoft.EntityFrameworkCore.Migrations;

namespace MutrajimAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceLanguage = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TargetLanguage = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    SourceLanguageName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TargetLanguageName = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
