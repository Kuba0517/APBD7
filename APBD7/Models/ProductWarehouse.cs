namespace APBD7.Models;

public class ProductWarehouse
{
    public int IdProductWarehouse { get; set; }
    public int Amount { get; set; }
    public double Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public int IdWarehouse { get; set; }
    public int IdProduct { get; set; }
    public int IdOrder { get; set; }

    public List<Warehouse> Warehouses { get; set; } = [];
    public List<Product> Products { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}