using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ABSUploadClient.Migrations
{
    public partial class UploadedDocumentTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UploadedDocumentID",
                table: "PaymentOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UploadedDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadTime = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    SourceValue = table.Column<string>(maxLength: 100, nullable: true),
                    FileName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedDocuments", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentOrders_UploadedDocuments_UploadedDocumentID",
                table: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "UploadedDocuments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentOrders_UploadedDocumentID",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "UploadedDocumentID",
                table: "PaymentOrders");
        }
    }
}
