using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ExcelDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(@"C:\Demos\YouTubeDemo.xlsx");

            var people = GetSetupData();

            await SaveExcelFile(people, file);

            List<PersonModel> peopleFromExcel = await LoadExcelFile(file);

            foreach (var p in peopleFromExcel)
            {
                Console.WriteLine($"{p.Id} {p.FirstName} {p.LastName}");
            }
        }

        private static async Task<List<PersonModel>> LoadExcelFile(FileInfo file)
        {
            List<PersonModel> output = new();

            using var package = new ExcelPackage(file);  //virtual excel file

            await package.LoadAsync(file);

            var ws = package.Workbook.Worksheets[0];

            int row = 3;
            int col = 1;

            while (string.IsNullOrWhiteSpace(ws.Cells[row,col].Value?.ToString()) == false)
            {
                PersonModel p = new();
                p.Id = int.Parse(ws.Cells[row, col].Value.ToString());
                p.FirstName = ws.Cells[row, col + 1].Value.ToString();
                p.LastName = ws.Cells[row, col + 2].Value.ToString();
                output.Add(p);
                row += 1;
            }

            return output;
        }

        private static async Task SaveExcelFile(List<PersonModel> people, FileInfo file)
        {
            DeleteIfExists(file);

            using var package = new ExcelPackage(file);

            var ws = package.Workbook.Worksheets.Add("MainReport"); //Adding new list

            var range = ws.Cells["A2"].LoadFromCollection(people, true); // Add range of objects
            range.AutoFitColumns();// If coloumn is widher scale it

            // Formats the header
            ws.Cells["A1"].Value = "Our Cool Report"; // Setting value
            ws.Cells["A1:C1"].Merge = true;  //Merge coloumns
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;  //Centring coloumn
            ws.Row(1).Style.Font.Size = 24;  //Coloumn font
            ws.Row(1).Style.Font.Color.SetColor(Color.Blue); //Coulomn color

            ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Row(2).Style.Font.Bold = true; //style
            ws.Column(3).Width = 20; //Set ciloumn width

            await package.SaveAsync();
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private static List<PersonModel> GetSetupData()
        {
            List<PersonModel> output = new()
            {
                new() { Id = 1, FirstName = "Tim", LastName = "Corey" },
                new() { Id = 2, FirstName = "Sue", LastName = "Storm" },
                new() { Id = 3, FirstName = "Jane", LastName = "Smith" }
            };

            return output;
        }
    }
}
