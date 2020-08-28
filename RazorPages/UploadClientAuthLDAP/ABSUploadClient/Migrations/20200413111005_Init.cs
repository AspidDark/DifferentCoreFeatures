using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ABSUploadClient.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleBriefs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accout = table.Column<string>(maxLength: 100, nullable: true),
                    ModuleValue = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleBriefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RequestRecivedOn = table.Column<DateTime>(type: "date", nullable: false),
                    CreditContractNumber = table.Column<string>(maxLength: 100, nullable: true),
                    Number = table.Column<string>(maxLength: 100, nullable: true),
                    IncomeDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    PayerName = table.Column<string>(maxLength: 1000, nullable: true),
                    Comment = table.Column<string>(maxLength: 200, nullable: true),
                    AuthentificationData = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ModuleBriefs",
                columns: new[] { "Id", "Accout", "ModuleValue" },
                values: new object[,]
                {
                    { 1, "40702810010310008965", "VTBO" },
                    { 2, "40702810210310028965", "VTBR" },
                    { 3, "40702810469000031377", "SBRF" },
                    { 4, "40702810310310000427", "VTBFK" },
                    { 5, "40701810710310008965", "VTBR2" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleBriefs");

            migrationBuilder.DropTable(
                name: "PaymentOrders");
        }
    }
}
