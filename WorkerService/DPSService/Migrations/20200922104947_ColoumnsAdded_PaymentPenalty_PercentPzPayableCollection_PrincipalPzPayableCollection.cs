using Microsoft.EntityFrameworkCore.Migrations;

namespace DPSService.Migrations
{
    public partial class ColoumnsAdded_PaymentPenalty_PercentPzPayableCollection_PrincipalPzPayableCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaymentPenalty",
                table: "GraceContacts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentPzPayableCollection",
                table: "GraceContacts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrincipalPzPayableCollection",
                table: "GraceContacts",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentPenalty",
                table: "GraceContacts");

            migrationBuilder.DropColumn(
                name: "PercentPzPayableCollection",
                table: "GraceContacts");

            migrationBuilder.DropColumn(
                name: "PrincipalPzPayableCollection",
                table: "GraceContacts");
        }
    }
}
