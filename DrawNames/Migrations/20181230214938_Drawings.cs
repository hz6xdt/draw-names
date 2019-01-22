using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrawNames.Migrations
{
    public partial class Drawings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DrawingId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Drawings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drawings", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Drawings_DrawingId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Drawings");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DrawingId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DrawingId",
                table: "AspNetUsers");
        }
    }
}
