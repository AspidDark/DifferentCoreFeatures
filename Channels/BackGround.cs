using System.Threading.Channels;

namespace Channels;

//2 publishers  1 reader
public class BackGroundSingle(ChannelReader<ChannelData> reader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in reader.ReadAllAsync(stoppingToken))
        {
            Console.WriteLine($"[Consumer 1] Processing Order: {message.ChannelName}");
            Console.WriteLine($"[Consumer 1] Processing Order: {message.StringData}");
        }
    }
}

//2 publishers  1 reader
public class BackGroundSingleWriter(ChannelWriter<ChannelData> writer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true) 
        {
            await writer.WriteAsync(new() { ChannelName = Guid.NewGuid().ToString(), StringData = "BackGround" });
            await Task.Delay(500);
        }
    }

}



//1 publiser  2 readers differnt messages processing
public class BackGroundManyReader4(Channel<ChannelData3> reader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in reader.Reader.ReadAllAsync(stoppingToken))
        {
            Console.WriteLine($"[Consumer 4] Processing Order: {message.ChannelName}");
            Console.WriteLine($"[Consumer 4] Processing Order: {message.StringData}");
        }
    }
}


//1 publiser  2 readers differnt messages processing
public class BackGroundManyReader3(Channel<ChannelData3> reader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in reader.Reader.ReadAllAsync(stoppingToken))
        {
            Console.WriteLine($"[Consumer 3] Processing Order: {message.ChannelName}");
            Console.WriteLine($"[Consumer 3] Processing Order: {message.StringData}");
        }
    }
}

