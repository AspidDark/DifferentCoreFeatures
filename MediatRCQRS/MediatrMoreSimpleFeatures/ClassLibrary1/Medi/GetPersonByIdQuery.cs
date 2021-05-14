using ClassLibrary1.Models;
using MediatR;

namespace ClassLibrary1.Medi
{
    public record GetPersonByIdQuery(int Id) : IRequest<Person>;
}
