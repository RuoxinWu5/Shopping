using Data.Model;
using Microsoft.EntityFrameworkCore;
using Data.Repository;
using Service;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

string connectionString = builder.Configuration.GetConnectionString("MyDb") ?? throw new ArgumentNullException(nameof(connectionString));
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<ShoppingDbContext>(options => options.UseMySql(connectionString, serverVersion));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();