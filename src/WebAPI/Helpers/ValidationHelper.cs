using Microsoft.AspNetCore.Mvc;

namespace NeoServer.Web.API.Helpers;

public static class ValidationHelper
{
    public static BadRequestObjectResult GetInvalidModelStateResponse(ActionContext context)
    {
        var modelState = context.ModelState;

        if (modelState.IsValid) return null;

        var errors = context.ModelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = new
        {
            Success = false,
            Data = new { },
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    }
}