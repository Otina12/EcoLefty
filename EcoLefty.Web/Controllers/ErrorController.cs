using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class ErrorController : Controller
{
    [HttpGet("Error/{statusCode:int}")]
    public IActionResult Error(int statusCode, string message = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetErrorTitle(statusCode),
            Detail = message ?? "An error occurred"
        };

        Response.StatusCode = statusCode;
        return View(problemDetails);
    }

    private string GetErrorTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            500 => "Server Error",
            _ => "Error"
        };
    }
}