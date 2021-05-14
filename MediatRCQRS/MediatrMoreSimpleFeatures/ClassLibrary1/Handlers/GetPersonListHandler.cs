using ClassLibrary1.Medi;
using ClassLibrary1.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1.Handlers
{
    public class GetPersonListHandler : IRequestHandler<GetPersonListQuery, List<Person>>
    {
        public Task<List<Person>> Handle(GetPersonListQuery request, CancellationToken cancellationToken)
        {
            List<Person> people = new List<Person>();
            people.Add(new Person("1111", "qqq"));
            people.Add(new Person("112", "qqq"));
            people.Add(new Person("133", "qqq"));
            return Task.FromResult(people);
        }
    }
}
