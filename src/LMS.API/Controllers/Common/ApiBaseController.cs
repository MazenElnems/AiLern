using LMS.API.Models;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Common;

public class ApiBaseController : ControllerBase
{
    public static ActionResult<ApiResponse> HandleResponse(ControllerBase controller, Result result)
    {
        if(!result.IsSuccess)
            return HandleFailureResponseInternal(controller, result);

        return HandleSuccessResponseInternal(controller, result);
    }

    public static ActionResult<ApiResponse> HandleResponse<T>(ControllerBase controller, Result<T> result)
    {
        if(!result.IsSuccess)
            return HandleFailureResponseInternal(controller, result);

        return HandleSuccessResponseInternal(controller, result, result.Value);
    }

    private static ActionResult<ApiResponse> HandleSuccessResponseInternal (ControllerBase controller, Result result, object? data = null)
    {
        var response = ApiResponse.Ok(result.Message, data);
        return controller.StatusCode(StatusCodes.Status200OK, ExtractResponseFields(response));
    }

    private static ActionResult<ApiResponse> HandleFailureResponseInternal(ControllerBase controller, Result result)
    {
        var error = result.Error;

        if(error == null)
            return controller.StatusCode(500, ApiResponse.InternalError("An unknown error occurred."));

        var response = error.Type switch
        {
            ErrorType.Validation => ApiResponse.BadRequest(result.ValidationErrors, result.Message),
            ErrorType.NotFound => ApiResponse.NotFound(error.Message),
            ErrorType.Unauthorized => ApiResponse.Unauthorized(error.Message),
            ErrorType.Forbidden => ApiResponse.Forbidden(error.Message),
            ErrorType.BusinessRule => ApiResponse.BadRequest(null, error.Message),
            _ => ApiResponse.InternalError("Unknown error occurred.")
        };

        return controller.StatusCode(response.StatusCode, ExtractResponseFields(response));
    }

    private static object ExtractResponseFields(ApiResponse response)
    {
        // Success Response
        if (response.Success)
            return new
            {
                response.Success,
                response.Message,
                response.Data,
                response.StatusCode
            };

        // Validation Error Response
        else if (response.Errors != null)
            return new
            {
                response.Success,
                response.Message,
                response.Errors,
                response.StatusCode
            };

        // Other Error Response
        return new
        {
            response.Success,
            response.Message,
            response.StatusCode
        };
    }
}
