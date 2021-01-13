using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DPSService.Migrations
{
    public partial class GraceEndDate_GracePeriodStartDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GraceEndDate",
                table: "SupplementaryAgreements");

            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodStartDate",
                table: "SupplementaryAgreements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GracePeriodStartDate",
                table: "SupplementaryAgreements");

            migrationBuilder.AddColumn<DateTime>(
                name: "GraceEndDate",
                table: "SupplementaryAgreements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
