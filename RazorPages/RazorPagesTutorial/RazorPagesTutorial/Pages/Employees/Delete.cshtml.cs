using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesTutorial.Models;
using RazorPagesTutorial.Services;

namespace RazorPagesTutorial.Pages.Employees
{
    public class DeleteModel : PageModel
    {
        private readonly IEmployeeRepository employeeRepository;

        public DeleteModel(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public IActionResult OnGet(int id)
        {
            Employee = employeeRepository.GetEmployee(id);

            if (Employee == null)
            {
                return RedirectToPage("/NotFound");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            Employee deletedEmployee = employeeRepository.Delete(Employee.Id);

            if (deletedEmployee == null)
            {
                return RedirectToPage("/NotFound");
            }

            return RedirectToPage("Index");
        }
    }
}