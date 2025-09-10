using CraneCalc.Domain.Models;

namespace CraneCalc.Web.ViewModels;

public class HomeServicesModel
{
    public List<CargoItem> CargoItems { get; set; } = [];

    public int Quantity { get; set; } = 0;
}