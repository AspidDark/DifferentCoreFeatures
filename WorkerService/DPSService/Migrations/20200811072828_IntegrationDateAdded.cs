using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DPSService.Migrations
{
    public partial class IntegrationDateAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ItegarionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEvents", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEvents");
        }
    }
}
