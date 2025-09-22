using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.Web.Controllers;

[Route("Error")]
public class ErrorController : Controller
{
    [Route("{statusCode:int}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        Response.StatusCode = statusCode;
        
        return statusCode switch
        {
            404 => View("NotFound"),
            _ => View("Error")
        };
    }

    [Route("")]
    public IActionResult Error()
    {
        return View("Error");
    }
}