using Microsoft.AspNetCore.Mvc;

namespace DIFeatures.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //+Imp1
        private IEnumerable<IHelloer> Helloers { get; set; }
        //-Imp1

        //+Imp2
        private readonly IContractService contractService;
        //-Imp2
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IEnumerable<IHelloer> helloers, Func<int, IContractService> serviceProvider)
        {
            //+Imp1
            Helloers = helloers;
            //-Imp1

            //+Imp2
            contractService = serviceProvider(1);
            //-Imp2
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            //+Imp1
            string resolverValue = "HelloerB";
            //With reflection
            var helloer = Helloers.FirstOrDefault(h => h.GetType().Name == resolverValue);
            var message = helloer?.SayHello();

            //No reflection
            var helloer2 = Helloers.FirstOrDefault(h => h.CurrentName == resolverValue);
            var message2 = helloer2?.SayHello();
            //-Imp1

            //+Imp2
            var tmp = contractService;
            //-Imp2


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}