using RazorPagesTutorial.Models;
using System.Collections.Generic;
using System.Linq;

namespace RazorPagesTutorial.Services
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Mary", Department = Dept.HR,
                    Email = "mary@pragimtech.com", PhotoPath="mary.png" },
                new Employee() { Id = 2, Name = "John", Department = Dept.IT,
                    Email = "john@pragimtech.com", PhotoPath="john.png" },
                new Employee() { Id = 3, Name = "Sara", Department = Dept.IT,
                    Email = "sara@pragimtech.com", PhotoPath="sara.png" },
                new Employee() { Id = 4, Name = "David", Department = Dept.Payroll,
                    Email = "david@pragimtech.com" },
            };
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == id);
        }

        public Employee Update(Employee updatedEmployee)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == updatedEmployee.Id);

            if(employee != null)
            {
                employee.Name = updatedEmployee.Name;
                employee.Email = updatedEmployee.Email;
                employee.Department = updatedEmployee.Department;
                employee.PhotoPath = updatedEmployee.PhotoPath;
            }

            return employee;
        }

        public Employee Add(Employee newEmployee)
        {
            newEmployee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(newEmployee);
            return newEmployee;
        }

        public Employee Delete(int id)
        {
            var employeeToDelete = _employeeList.FirstOrDefault(e => e.Id == id);

            if(employeeToDelete != null)
            {
                _employeeList.Remove(employeeToDelete);
            }

            return employeeToDelete;
        }

        public IEnumerable<DeptHeadCount> EmployeeCountByDept(Dept? dept)
        {
            IEnumerable<Employee> query = _employeeList;

            if (dept.HasValue)
            {
                query = query.Where(e => e.Department == dept.Value);
            }

            return query.GroupBy(e => e.Department)
                                .Select(g => new DeptHeadCount()
                                {
                                    Department = g.Key.Value,
                                    Count = g.Count()
                                }).ToList();
        }

        public IEnumerable<Employee> Search(string searchTerm = null)
        {
            if(string.IsNullOrEmpty(searchTerm))
            {
                return _employeeList;
            }

            return _employeeList.Where(e => e.Name.Contains(searchTerm) ||
                                            e.Email.Contains(searchTerm)).ToList();
        }
    }
}