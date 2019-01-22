using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawNames.Migrations
{
    public partial class Drawing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DrawnUserId",
                table: "DrawingUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Drawn",
                table: "Drawings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrawnUserId",
                table: "DrawingUsers");

            migrationBuilder.DropColumn(
                name: "Drawn",
                table: "Drawings");
        }
    }
}
