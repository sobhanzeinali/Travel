using MediatR;
using Microsoft.Extensions.Logging;

namespace Travel.Application.Common.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private ILogger _logger;

    public UnhandledExceptionBehavior(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            var requestName = nameof(TRequest);
            _logger.LogError(e, "Travel Request: Unhandled Exception for Request {Name} {@Request}", requestName,
                request);
            throw;
        }
    }
}