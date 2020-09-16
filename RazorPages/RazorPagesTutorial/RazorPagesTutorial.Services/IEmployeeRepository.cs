using RazorPagesTutorial.Models;
using System;
using System.Collections.Generic;

namespace RazorPagesTutorial.Services
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployees();
        Employee GetEmployee(int id);
        Employee Update(Employee updatedEmployee);
        Employee Add(Employee newEmployee);
        Employee Delete(int id);
        IEnumerable<DeptHeadCount> EmployeeCountByDept(Dept? dept);
        IEnumerable<Employee> Search(string searchTerm);
    }
}
