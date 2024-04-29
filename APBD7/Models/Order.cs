
namespace APBD7.Models;

public class Order
{
    public int IdOrder { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }


    public List<Product> Products { get; set; }

}