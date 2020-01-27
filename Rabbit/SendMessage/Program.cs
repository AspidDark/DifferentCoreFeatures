using MassTransit;
using RabbitMQBase;
using System;
using System.Threading.Tasks;

namespace SendMessage
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

            });

            bus.Start();
            await bus.Publish(new Message(1, "New Message", DateTime.Now));
            Console.WriteLine("Sending Message");
            Console.ReadKey();

            bus.Stop();

        }
    }
}
