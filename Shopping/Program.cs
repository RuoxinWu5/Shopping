using Data.Model;
using Microsoft.EntityFrameworkCore;
using Data.Repository;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("MyDb") ?? throw new ArgumentNullException(nameof(connectionString));
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<ShoppingDbContext>(options => options.UseMySql(connectionString, serverVersion));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddScoped<IBuyerService, BuyerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();