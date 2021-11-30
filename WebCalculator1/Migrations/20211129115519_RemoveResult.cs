using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCalculator1.Migrations
{
    public partial class RemoveResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "Operations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
