using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawNames.Migrations
{
    public partial class DrawingUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Drawings_DrawingId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DrawingId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DrawingId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "DrawingUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DrawingId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawingUsers");

            migrationBuilder.AddColumn<int>(
                name: "DrawingId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DrawingId",
                table: "AspNetUsers",
                column: "DrawingId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Drawings_DrawingId",
                table: "AspNetUsers",
                column: "DrawingId",
                principalTable: "Drawings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
