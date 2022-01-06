using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Travel.Application.Common.Behaviors;

public class LoggingBehaviors<TRequest> : IRequestPreProcessor<TRequest>
{
    private readonly ILogger _logger;

    public LoggingBehaviors(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Travel Request : {@Request}", requestName, request);
    }
}