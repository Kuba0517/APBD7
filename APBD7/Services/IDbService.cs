using APBD7.DTOs;
using APBD7.Models;

namespace APBD7.Services;

public interface IDbService
{
    Task<Product?> GetProductById(int id);
    Task<Warehouse?> GetWarehouseById(int id);
    Task<Order?> GetOrderByIdAndAmount(int id, int amount);
    Task<ProductWarehouse?> GetProductWarehouseByOrder(int id);
    Task UpdateOrderFulfilledAt(int id);
    Task<int> AddProductWarehouse(ProductWarehouseDTO productWarehouseDto);
    Task<IEnumerable<ProductWarehouse>> GetProductWarehouses();
}