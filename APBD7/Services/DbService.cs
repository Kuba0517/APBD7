using System.Data;
using System.Data.SqlClient;
using APBD7.Models;
using Dapper;

namespace APBD7.Services;



public class DbService(IConfiguration configuration) : IDbService
{

    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    public async Task<Product?> GetProductById(int id)
    {
        await using var connection = await GetConnection();

        var products = await connection.QueryAsync<Product>(
            @"SELECT * FROM PRODUCT WHERE IdProduct = @Id",
            new {Id = id}
        );

        return products.FirstOrDefault();
    }

    public Task<Warehouse?> GetWarehouseById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> GetOrderByIdAndAmount(int id, int amount)
    {
        throw new NotImplementedException();
    }

    public Task<ProductWarehouse?> GetProductWarehouseByOrder(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<Order> UpdateOrderFulfilledAt(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<ProductWarehouse> AddProductWarehouse(ProductWarehouse productWarehouse)
    {
        throw new NotImplementedException();
    }
}