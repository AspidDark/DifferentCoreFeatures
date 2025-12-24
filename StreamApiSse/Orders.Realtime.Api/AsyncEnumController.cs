using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static Orders.Realtime.Api.AsyncEnumController;

namespace Orders.Realtime.Api;

[ApiController]
[Route("[controller]")]
public class AsyncEnumController : ControllerBase
{
    [HttpGet("sse")]
    public IResult Sse()
            => Results.ServerSentEvents(GetEvents(HttpContext.RequestAborted));

    private static async IAsyncEnumerable<string> GetEvents(
        [EnumeratorCancellation] CancellationToken ct)
    {
        for (var i = 0; i < 10; i++)
        {
            if (ct.IsCancellationRequested)
                yield break;

            yield return $"Event {i} at {DateTime.Now:O}";
            await Task.Delay(1000, ct);
        }
    }

    public sealed record TickDto(int Index, DateTime TimeUtc, string Message);

    [HttpGet("ticks")]
    [Produces("application/json")]
    public async IAsyncEnumerable<TickDto> GetTicks(
   [EnumeratorCancellation] CancellationToken ct)
    {
        for (var i = 0; i < 15; i++)
        {
            ct.ThrowIfCancellationRequested();

            yield return new TickDto(
                Index: i,
                TimeUtc: DateTime.UtcNow,
                Message: $"Tick {i}");

            await Task.Delay(1000, ct);
        }
    }
}

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        using var client = new HttpClient();

//        using var response = await client.GetAsync(
//            "http://localhost:5226/WeatherForecast/ticks",
//            HttpCompletionOption.ResponseHeadersRead);

//        response.EnsureSuccessStatusCode();

//        await using var stream = await response.Content.ReadAsStreamAsync();

//        var jsonOptions = new JsonSerializerOptions
//        {
//            PropertyNameCaseInsensitive = true
//        };

//        await foreach (var tick in JsonSerializer.DeserializeAsyncEnumerable<TickDto>(stream, jsonOptions))
//        {
//            if (tick is null) continue;
//            Console.WriteLine($"{tick.Index}: {tick.Message}");
//        }
//    }
//}


