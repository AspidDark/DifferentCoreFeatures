using ClassLibrary1.Models;
using MediatR;
using System.Collections.Generic;

namespace ClassLibrary1.Medi
{
    public record GetPersonListQuery() : IRequest<List<Person>>;
}
