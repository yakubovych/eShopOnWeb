using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Web.ViewModels;

public class ReservedOrder
{
    public Address ShipAddress { get; set; }
    public decimal FinalPrice { get; set; }
    public List<string> Items { get; set; } = new List<string>();
}
