using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class CustomersService : Customer.CustomerBase
    {
        private readonly ILogger<CustomersService> _logger;
        public CustomersService(ILogger<CustomersService> logger)
        {
            _logger = logger;
        }

        public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModel request, ServerCallContext context)
        {
            CustomerModel output = new CustomerModel();
            switch (request.UserId)
            {
                case 1:
                    output.FirstName = "FirstName1";
                    output.LastName = "LastName1";
                    break;
                case 2:
                    output.FirstName = "FirstName2";
                    output.LastName = "LastName2";
                    output.IsAlive = false;
                    output.Age = 7;
                    break;
                default:
                    output.FirstName = "FirstName3";
                    output.LastName = "LastName3";
                    
                    break;

            }
            return Task.FromResult(output);
           // return base.GetCustomerInfo(request, context);
        }



        public override async Task GetNewCustomers(NewCustomerRequest request, 
            IServerStreamWriter<CustomerModel> responseStream, ServerCallContext context)
        {
            List<CustomerModel> customers = new List<CustomerModel>
            {
                new CustomerModel
                {
                    FirstName="FirstName",
                    LastName="LastName",
                    EmailAddress="EmailAddress"
                },
                 new CustomerModel
                {
                    FirstName="FirstName2",
                    LastName="LastName2",
                    EmailAddress="EmailAddress2"
                },
                  new CustomerModel
                {
                    FirstName="FirstName3",
                    LastName="LastName3",
                    EmailAddress="EmailAddress3"
                }
            };

            foreach (var cust in customers)
            {
                await responseStream.WriteAsync(cust);
            }
            //return base.GetNewCustomers(request, responseStream, context);
        }
    }
}
