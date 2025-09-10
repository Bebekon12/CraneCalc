using CraneCalc.Web.Mocks;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.Web.Controllers;

[Route("Calculation")]
public class CalculationController : Controller
{
    [Route("Index/{id:int}")]
    public IActionResult Index(int id)
    {
        var request = MockData.RequestCargoItemsM[3];
        
        return View(request);
    }
}