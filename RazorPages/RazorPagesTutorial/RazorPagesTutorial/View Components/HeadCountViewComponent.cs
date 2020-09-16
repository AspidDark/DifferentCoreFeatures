using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RazorPagesTutorial.Models;
using RazorPagesTutorial.Services;

namespace RazorPagesTutorial.ViewComponents
{
    public class HeadCountViewComponent : ViewComponent
    {
        private readonly IEmployeeRepository employeeRepository;

        public HeadCountViewComponent(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public IViewComponentResult Invoke(Dept? department = null)
        {
            var result = employeeRepository.EmployeeCountByDept(department);
            return View(result);
        }
    }
}