using ClassLibrary1.Medi;
using ClassLibrary1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator; //no need of implementation

        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/<PersonController>/5
        [HttpGet("{id}")]
        public async Task<Person> Get(int id)
        {
            return await _mediator.Send(new GetPersonByIdQuery(id));
        }

        [HttpGet]
        public async Task<List<Person>> Get()
        {
            return await _mediator.Send(new GetPersonListQuery());
        }
    }
}
