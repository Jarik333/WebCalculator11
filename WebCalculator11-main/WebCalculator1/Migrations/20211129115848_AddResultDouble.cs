using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCalculator1.Migrations
{
    public partial class AddResultDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Result",
                table: "Operations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "Operations");
        }
    }
}
