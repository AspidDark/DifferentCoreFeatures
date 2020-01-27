using System.Collections;

namespace LinqLib.Additional
{
    public class Employee2
    {
        public string id;
        public string firstName;
        public string lastName;

        public static ArrayList GetEmployeesArrayList()
        {
            ArrayList al = new ArrayList();

            al.Add(new Employee2 { id = "1", firstName = "Joe", lastName = "Rattz" });
            al.Add(new Employee2 { id = "2", firstName = "William", lastName = "Gates" });
            al.Add(new Employee2 { id = "3", firstName = "Anders", lastName = "Hejlsberg" });
            al.Add(new Employee2 { id = "4", firstName = "David", lastName = "Lightman" });
            al.Add(new Employee2 { id = "101", firstName = "Kevin", lastName = "Flynn" });
            return (al);
        }

        public static Employee2[] GetEmployeesArray()
        {
            return ((Employee2[])GetEmployeesArrayList().ToArray(typeof(Employee2)));
        }
    }
}
