using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibraryManagement.API.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid) // Check if model state is valid (based on Data Annotations or FluentValidation)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var errorResponse = new
            {
                errors = errors // Dictionary of property names and error messages
            };

            context.Result = new BadRequestObjectResult(errorResponse); // Return 400 with validation errors
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action after execution in this validation filter example
    }
}
