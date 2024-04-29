using APBD7.DTOs;
using APBD7.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("products/{id:int}", (int id, IConfiguration configuration, IDbService service) => service.GetProductById(id));
    

app.MapPost("add", (RequestDTO request, IConfiguration configuration) =>
{
    

});

app.Run();