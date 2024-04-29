using System.Data;
using System.Data.SqlClient;
using APBD7.DTOs;
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

    public async Task<Warehouse?> GetWarehouseById(int id)
    {
        await using var connection = await GetConnection();

        var warehouses = await connection.QueryAsync<Warehouse>(
            "SELECT * FROM WAREHOUSE WHERE IdWarehouse = @Id",
            new { Id = id }
        );

        return warehouses.FirstOrDefault();

    }

    public async Task<Order?> GetOrderByIdAndAmount(int id, int amount)
    {
        await using var connection = await GetConnection();

        var orders = await connection.QueryAsync<Order>(
            "SELECT * FROM [ORDER] WHERE IdOrder = @Id AND Amount = @Amount",
            new
            {
                Id = id,
                Amount = amount
            }
        );

        return orders.FirstOrDefault();

    }

    public async Task<ProductWarehouse?> GetProductWarehouseByOrder(int orderId)
    {
        await using var connection = await GetConnection();

        var productWarehouses = await connection.QueryAsync<ProductWarehouse>(
            "SELECT * FROM PRODUCT_WAREHOUSE WHERE IdOrder = @Id",
            new
            {
                Id = orderId,
            }
        );

        return productWarehouses.FirstOrDefault();

    }

    public async Task UpdateOrderFulfilledAt(int id)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {

            await connection.ExecuteAsync(
                @"UPDATE [ORDER] SET FulfilledAt = @Date WHERE IdOrder = @Id",
                new
                {
                    Date = DateTime.Now,
                    Id = id
                },
                transaction: transaction

            );

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> AddProductWarehouse(ProductWarehouseDTO productWarehouse)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            var productWarehouseId = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO PRODUCT_WAREHOUSE VALUES (@IdWarehouse, @IdProduct, @IdOrder, 
                                      @Amount, @Price, @CreatedAt);
                        SELECT cast(scope_identity() as int)",
                new
                {
                    IdWarehouse = productWarehouse.IdWarehouse,
                    IdProduct = productWarehouse.IdProduct,
                    IdOrder = productWarehouse.IdOrder,
                    Amount = productWarehouse.Amount,
                    Price = (await GetProductById(productWarehouse.IdProduct)).Price * productWarehouse.Amount,
                    CreatedAt = DateTime.Now
                },
                transaction: transaction

            );

            await transaction.CommitAsync();

            return productWarehouseId;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
       
    }

    public async Task<IEnumerable<ProductWarehouse>> GetProductWarehouses()
    {
        await using var connection = await GetConnection();

        var productWarehouses = await connection.QueryAsync<ProductWarehouse>(
            "SELECT * FROM PRODUCT_WAREHOUSE"
        );

        return productWarehouses;
        
    }
}