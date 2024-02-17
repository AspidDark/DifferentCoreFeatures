using Grpc.Net.ClientFactory;
using GrpcClient;

namespace Client
{
    public interface IService 
    {
        Task DoSmth();
    }

    public class Service : IService
    {
        private readonly Greeter.GreeterClient _client;
        public Service(GrpcClientFactory grpcClientFactory)
        {
            _client = grpcClientFactory.CreateClient<Greeter.GreeterClient>("Greeter1");
        }


        public async Task DoSmth()
        {
            HelloRequest request = new HelloRequest 
            { 
                Name="Tim san"
            };
            var Results = _client.SayHello(request);
        }
    }
}
