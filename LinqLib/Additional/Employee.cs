using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LinqLib.Additional
{
    public class Employee
    {
        public int id;
        public string firstName;
        public string lastName;

        public static ArrayList GetEmployeesArrayList()
        {
            ArrayList al = new ArrayList();

            al.Add(new Employee { id = 1, firstName = "Joe", lastName = "Rattz" });
            al.Add(new Employee { id = 2, firstName = "William", lastName = "Gates" });
            al.Add(new Employee { id = 3, firstName = "Anders", lastName = "Hejlsberg" });
            al.Add(new Employee { id = 4, firstName = "David", lastName = "Lightman" });
            al.Add(new Employee { id = 101, firstName = "Kevin", lastName = "Flynn" });
            return (al);
        }

        public static Employee[] GetEmployeesArray()
        {
            return ((Employee[])GetEmployeesArrayList().ToArray(typeof(Employee)));
        }
    }
}
