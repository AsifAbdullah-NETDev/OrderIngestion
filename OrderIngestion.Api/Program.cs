using Microsoft.OpenApi.Models;
using Npgsql;
using OrderIngestion.Api.Swagger;
using OrderIngestion.Application.Models;
using OrderIngestion.Application.Services;
using OrderIngestion.Infrastructure.Data;
using OrderIngestion.Infrastructure.Extensions;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Ingestion API",
        Version = "v1",
        Description = "API for managing orders"
    });
    c.SchemaFilter<DTOsSchemaFilter>();
});

//builder.Services.AddSingleton<DapperContext>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => "Order Ingestion API is running!");
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapControllers();

app.Run();
