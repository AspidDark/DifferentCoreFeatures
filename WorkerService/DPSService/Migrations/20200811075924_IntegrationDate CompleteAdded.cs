using Microsoft.EntityFrameworkCore.Migrations;

namespace DPSService.Migrations
{
    public partial class IntegrationDateCompleteAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Complete",
                table: "IntegrationEvents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complete",
                table: "IntegrationEvents");
        }
    }
}
