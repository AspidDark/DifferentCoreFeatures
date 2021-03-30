using System;

namespace MongoDB
{

    //https://www.youtube.com/watch?v=69WBy4MHYUw&t=154s   54
    class Program
    {
        static void Main(string[] args)
        {

            MongoCRUD crud = new("AddressBook");

            Person person = new()
            {
                FirstName = "Tim",
                LastName = "Au",
                PrimaryAddress = new()
                {
                    StreetAddress = "aaaa",
                    City = "bbbb",
                    State = "cccc",
                    ZipCode = "87876"
                }
            };

            crud.InsertRecord<Person>("Users", person);
            Console.WriteLine("Hello World!");
        }
    }
}
