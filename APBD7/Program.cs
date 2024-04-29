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

app.MapPost("add-product", async (RequestDTO request, IConfiguration configuration, IDbService service, 
    IValidator<RequestDTO> validator) =>
{
    var validate = await validator.ValidateAsync(request);
    if (!validate.IsValid) return Results.ValidationProblem(validate.ToDictionary());
    var product = await service.GetProductById(request.IdProduct);
    if (product == null) return Results.NotFound();

    var warehouse = await service.GetWarehouseById(request.IdWarehouse);
    if (warehouse == null) return Results.NotFound();
    
    var order = await service.GetOrderByIdAndAmount(request.IdProduct, request.Amount);
    if (order == null) return Results.NotFound();
    
    if (request.CreatedAt < order.CreatedAt) return Results.NotFound();
    
    if (await service.GetProductWarehouseByOrder(order.IdOrder) != null) return Results.BadRequest();
    
    await service.UpdateOrderFulfilledAt(order.IdOrder);

    var result = await service.AddProductWarehouse(
        new ProductWarehouseDTO(warehouse.IdWarehouse, product.IdProduct, order.IdOrder, request.Amount)
    );

    Console.WriteLine(result);

    return Results.Created($"created product warehouse with id: {result}", result);

});

app.Run();