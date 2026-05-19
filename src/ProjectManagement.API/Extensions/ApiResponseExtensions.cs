using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Common.Models;

namespace ProjectManagement.API.Extensions;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.Succeeded)
            return new OkObjectResult(ApiResponse<object>.Ok(null!, result.Message));

        return new BadRequestObjectResult(ApiResponse<object>.Fail(result.Errors, result.Message));
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.Succeeded)
            return new OkObjectResult(ApiResponse<T>.Ok(result.Data!, result.Message));

        return new BadRequestObjectResult(ApiResponse<T>.Fail(result.Errors, result.Message));
    }
}
