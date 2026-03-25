using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LMS.Application.Common.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
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