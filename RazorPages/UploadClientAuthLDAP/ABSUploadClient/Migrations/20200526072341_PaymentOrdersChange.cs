using Microsoft.EntityFrameworkCore.Migrations;

namespace ABSUploadClient.Migrations
{
    public partial class PaymentOrdersChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_UploadedDocuments_UploadedDocumentID",
                table: "PaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrders_UploadedDocumentID",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "UploadedDocumentID",
                table: "PaymentOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UploadedDocumentID",
                table: "PaymentOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_UploadedDocumentID",
                table: "PaymentOrders",
                column: "UploadedDocumentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentOrders_UploadedDocuments_UploadedDocumentID",
                table: "PaymentOrders",
                column: "UploadedDocumentID",
                principalTable: "UploadedDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
