using System;
using RabbitMQBase;
using MassTransit;

namespace ReciveMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc=>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                sbc.ReceiveEndpoint(host, "new_queue", ep =>
                {
                    //ep.Handler<Message>(context =>
                    //{
                    //    return Console.Out.WriteLineAsync($"Recived: {context.Message.Name}");
                    //});
                    ep.Consumer<MessageConsumer>();
                });
        
            });
            bus.Start();

            Console.WriteLine("Recive message");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
