using APBD7.DTOs;
using APBD7.Services;
using APBD7.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbService, DbService>();

builder.Services.AddValidatorsFromAssemblyContaining<RequestDTOValidators>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("add-product", async (RequestDTO request, IConfiguration configuration, IDbService service, IValidator<RequestDTO> validator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid) 
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var product = await service.GetProductById(request.IdProduct);
    if (product == null) 
    {
        return Results.NotFound("Product not found.");
    }

    var warehouse = await service.GetWarehouseById(request.IdWarehouse);
    if (warehouse == null) 
    {
        return Results.NotFound("Warehouse not found.");
    }
    
    var order = await service.GetOrderByIdAndAmount(request.IdProduct, request.Amount);
    if (order == null || request.CreatedAt < order.CreatedAt) 
    {
        return Results.NotFound("Order not found or request date is earlier than order date.");
    }
    
    if (await service.GetProductWarehouseByOrder(order.IdOrder) != null) 
    {
        return Results.BadRequest("Product already exists in the warehouse for this order.");
    }

    await service.UpdateOrderFulfilledAt(order.IdOrder);

    var productWarehouseId = await service.AddProductWarehouse(
        new ProductWarehouseDTO(warehouse.IdWarehouse, product.IdProduct, order.IdOrder, request.Amount)
    );

    Console.WriteLine($"Product warehouse created with ID: {productWarehouseId}");

    return Results.Created($"Product warehouse created with ID: {productWarehouseId}", productWarehouseId);
});


app.Run();