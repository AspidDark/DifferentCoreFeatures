using JasperFx.Core;
using MessagingContracts;

namespace MoviesConsumer.Handlers;

public class MovieCreatedHandler
{
    private readonly ILogger<MovieCreatedHandler> _logger;

    public MovieCreatedHandler(ILogger<MovieCreatedHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(MovieCreated movieCreated)
    {
        _logger.LogInformation(movieCreated.ToString());
    }
}
