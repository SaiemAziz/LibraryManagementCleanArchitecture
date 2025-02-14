using System;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Utilities;

public static class APIResponseHelper
{
    public static IActionResult CreateSuccessResponse<T>(T data, string? message = null)
    {
        return new OkObjectResult(new
        {
            success = true,
            message = message,
            data = data
        });
    }

    public static IActionResult CreateErrorResponse(string errorMessage, int statusCode = 400)
    {
        return new ObjectResult(new
        {
            success = false,
            error = errorMessage
        })
        {
            StatusCode = statusCode
        };
    }
}