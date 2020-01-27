using MassTransit;
using RabbitMQBase;
using System;
using System.Threading.Tasks;

namespace ReciveMessage
{
    public class MessageConsumer : IConsumer<Message>
    {
        public async Task Consume(ConsumeContext<Message> context)
        {
            await Console.Out.WriteLineAsync($"Recived: {context.Message.Name}");
        }
    }
}
