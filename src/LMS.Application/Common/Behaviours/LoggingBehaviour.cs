using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Common.Behaviours;

internal class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Handling {RequestName}",
            requestName
        );

        var response = await next();

        if(response is IResult result)
        {

            if (!result.IsSuccess)
            {

                if (result?.Error?.Type == ErrorType.Validation)
                {
                    _logger.LogWarning(
                        "Handled {RequestName} FAILED due to validation fauiler {@ValidationErrors}",
                        requestName,
                        result.ValidationErrors
                    );
                }
                else
                {
                    _logger.LogWarning(
                            "Handled {RequestName} FAILED with error {@Error}",
                            requestName,
                            result?.Error
                    );
                }
            }
            else
            {
                _logger.LogInformation(
                    "Handled {RequestName} SUCCESS",
                    requestName
                );
            }
        }

        return response;
    }
}