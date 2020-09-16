using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorPagesTutorial.Services.Migrations
{
    public partial class spGetEmployeeById : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Create Procedure spGetEmployeeById
                            @Id int
                            as
                            Begin
	                            Select * from Employees
	                            Where Id = @Id
                            End";
            migrationBuilder.Sql(procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"Drop procedure spGetEmployeeById";
            migrationBuilder.Sql(procedure);
        }
    }
}
