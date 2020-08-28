using Microsoft.EntityFrameworkCore.Migrations;

namespace ABSUploadClient.Migrations
{
    public partial class BindingErrorMessageAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BindingErrorMessage",
                table: "PaymentBindings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BindingErrorMessage",
                table: "PaymentBindings");
        }
    }
}
