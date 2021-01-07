using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCoreLogging.Pages
{
    public class IndexModel : PageModel
    {
        //// Standart way
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}
        private readonly ILogger _logger;
        public IndexModel(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger("Name of Category");
        }

        public const string message = "Message";
        public void OnGet()
        {
            _logger.LogInformation(LoggingId.DemoCode, "Message"); //show also id of log information
            //Logging Level from least significant to most significant
            _logger.LogTrace(message); // Debugging
            _logger.LogDebug(message); // Debugging
            _logger.LogInformation(message); // Flow of application using
            _logger.LogWarning(message); // Often User mistakes
            _logger.LogError(message); // Unexpected behavior
            _logger.LogCritical(message); // Application is crushing or we loosing data

            _logger.LogError("{Param1}The server went down temprerorali at {Param2}", " Server Name ", DateTime.UtcNow); //Semantic or structure logging ()=> good for filtering ... elactic search
        }
    }
}
