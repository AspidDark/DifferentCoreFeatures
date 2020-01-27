using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var input = new HelloRequest {Name="Tim" };
            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            //var client = new Greeter.GreeterClient(channel);
            //var replay = await client.SayHelloAsync(input);
            //Console.WriteLine(replay.Message);

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var customerClient = new Customer.CustomerClient(channel);
            var clientRequested = new CustomerLookupModel { UserId = 2 };
            var replay = await customerClient.GetCustomerInfoAsync(clientRequested);
            //Console.WriteLine(replay);
            //Console.WriteLine($"{replay.FirstName}  {replay.LastName}");

            using (var call = customerClient.GetNewCustomers(new NewCustomerRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var current = call.ResponseStream.Current;
                    Console.WriteLine(current);
                }
            }
                Console.ReadLine();
        }
    }
}
