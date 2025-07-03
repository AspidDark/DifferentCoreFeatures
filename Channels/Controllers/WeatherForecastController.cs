using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace Channels.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(Channel<ChannelData> channel, Channel<ChannelData3> channel3) : ControllerBase
{

    //[HttpGet(Name = "TwoPublisersOneReder")]
    //public async Task Get1()
    //{
    //   await channel.Writer.WriteAsync(new() {ChannelName = Guid.NewGuid().ToString(), StringData = "FromController1" });
    //}



    [HttpGet(Name = "OnePubliserWhoReaderDifferntMessages")]
    public async Task Get3()
    {
        await channel3.Writer.WriteAsync(new() { ChannelName = Guid.NewGuid().ToString(), StringData = "FromController3" });
    }
}
