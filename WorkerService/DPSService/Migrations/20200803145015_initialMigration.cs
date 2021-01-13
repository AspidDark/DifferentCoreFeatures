using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DPSService.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<Guid>(nullable: false),
                    AccountID = table.Column<string>(nullable: true),
                    DateAccountOpened = table.Column<DateTime>(nullable: false),
                    DateOfLastPayment = table.Column<DateTime>(nullable: false),
                    AccountRating = table.Column<string>(maxLength: 2, nullable: true),
                    DateAccountRating = table.Column<DateTime>(nullable: false),
                    ContractAmount = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    PastDue = table.Column<decimal>(nullable: false),
                    NextPaymentAmount = table.Column<decimal>(nullable: false),
                    MOP = table.Column<string>(maxLength: 10, nullable: true),
                    DateofContractTermination = table.Column<DateTime>(nullable: false),
                    DatePaymentDue = table.Column<DateTime>(nullable: false),
                    DateInterestPaymentDue = table.Column<DateTime>(nullable: false),
                    AmountOutstanding = table.Column<decimal>(nullable: false),
                    CompleteObligationsDate = table.Column<DateTime>(nullable: false),
                    PrincipalAmountOutstanding = table.Column<string>(nullable: true),
                    InterestAmountOutstanding = table.Column<string>(nullable: true),
                    OtherAmountOutstanding = table.Column<string>(nullable: true),
                    PrincipalAmountPastDue = table.Column<string>(nullable: true),
                    InterestAmountPastDue = table.Column<string>(nullable: true),
                    OtherAmountPastDue = table.Column<string>(nullable: true),
                    GracePeriodStartDate = table.Column<DateTime>(nullable: false),
                    GracePeriodEndDate = table.Column<DateTime>(nullable: false),
                    DateGracePeriodDeclined = table.Column<DateTime>(nullable: false),
                    GracePeriodReason = table.Column<string>(maxLength: 10, nullable: true),
                    CreditCredDateMissedpayout = table.Column<DateTime>(nullable: false),
                    OpCredSumPayout = table.Column<string>(nullable: true),
                    OpCredSumPaid = table.Column<decimal>(nullable: false),
                    OpCredDateOverdue = table.Column<DateTime>(nullable: false),
                    OpCredSumNextpayout = table.Column<string>(nullable: true),
                    OpCredDateNextpayout = table.Column<DateTime>(nullable: false),
                    TaCredSumPayout = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<Guid>(nullable: false),
                    ContractClosedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraceContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<Guid>(nullable: false),
                    GracePeriodStartDate = table.Column<DateTime>(nullable: false),
                    GracePeriodStatus = table.Column<bool>(nullable: false),
                    PrincipalPayable = table.Column<decimal>(nullable: false),
                    BankingServiceType = table.Column<string>(maxLength: 250, nullable: true),
                    BankingServiceName = table.Column<string>(maxLength: 250, nullable: true),
                    LoanPeriodDaily = table.Column<int>(nullable: false),
                    LoanRateDay = table.Column<decimal>(nullable: false),
                    TermVariance = table.Column<int>(nullable: false),
                    ContractSignedOn = table.Column<DateTime>(nullable: false),
                    ContractNumber = table.Column<string>(nullable: true),
                    UpdateDTTM = table.Column<DateTime>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    GracePeriodEndDate = table.Column<DateTime>(nullable: false),
                    ContractNumberABS = table.Column<string>(maxLength: 50, nullable: true),
                    PrincipalPzPayable = table.Column<decimal>(nullable: false),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    PercentPzPayable = table.Column<decimal>(nullable: false),
                    PenaltyPzPayable = table.Column<decimal>(nullable: false),
                    ContractStatus = table.Column<int>(nullable: false),
                    TotalPercentPzPaid = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    TotalPercentCharge = table.Column<decimal>(nullable: false),
                    TotalPercentPzPayable = table.Column<decimal>(nullable: false),
                    OldDateClosedContract = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraceContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestCharges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<Guid>(nullable: false),
                    ContractNumberABS = table.Column<string>(maxLength: 50, nullable: true),
                    ContractType = table.Column<int>(nullable: false),
                    ContractKind = table.Column<int>(nullable: false),
                    ContractAmount = table.Column<decimal>(nullable: false),
                    LastDayOfMonth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestCharges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplementaryAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<Guid>(nullable: false),
                    ContractNumberABS = table.Column<string>(maxLength: 50, nullable: true),
                    GraceEndDate = table.Column<DateTime>(nullable: false),
                    NewContractEndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplementaryAgreements", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationResults");

            migrationBuilder.DropTable(
                name: "ContractData");

            migrationBuilder.DropTable(
                name: "GraceContacts");

            migrationBuilder.DropTable(
                name: "InterestCharges");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SupplementaryAgreements");
        }
    }
}
