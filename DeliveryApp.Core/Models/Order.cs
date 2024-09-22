namespace DeliveryApp.Core.Models;

public class Order
{
    public int Id { get; set; }
    public string ProductRowKey { get; set; }
    public string Email { get; set; }

    public string ProductName { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
 
}
