using Microsoft.EntityFrameworkCore.Migrations;

namespace ABSUploadClient.Migrations
{
    public partial class UploadedDocumentTableBackupFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackupFileName",
                table: "UploadedDocuments",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackupFileName",
                table: "UploadedDocuments");
        }
    }
}
