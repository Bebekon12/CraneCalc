using CraneCalc.Domain.Models;

namespace CraneCalc.Web.ViewModels;

public class HomeServicesModel
{
    public List<Cargo> CargoItems { get; set; } = [];

    public Guid? CartId { get; set; }

    public int Quantity { get; set; } = 0;
}