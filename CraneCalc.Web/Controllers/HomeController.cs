using CraneCalc.Web.Mocks;
using CraneCalc.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Services(string? search)
    {
        var items = MockData.ItemsList;
        var count = MockData.RequestCargoItemsM[3].Count;

        if (!string.IsNullOrEmpty(search))
            items = items.Where(i => i.Type.Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToList();

        var model = new HomeServicesModel
        {
            CargoItems = items,
            Quantity = count,
        };

        ViewBag.CurrentTitle = search;
        return View(model);
    }
    
    public IActionResult CargoDetail(Guid id)
    {
        var items = MockData.ItemsList;
        var element = items.FirstOrDefault(e => e.Id == id);
        return View(element);
    }
}