using APBD7.Models;

namespace APBD7.Services;

public interface IDbService
{
    Task<Product?> GetProductById(int id);
    Task<Warehouse?> GetWarehouseById(int id);
    Task<Order?> GetOrderByIdAndAmount(int id, int amount);
    Task<ProductWarehouse?> GetProductWarehouseByOrder(Order order);
    Task<Order> UpdateOrderFulfilledAt(Order order);
    Task<ProductWarehouse> AddProductWarehouse(ProductWarehouse productWarehouse);
}