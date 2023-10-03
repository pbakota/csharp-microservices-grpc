using Delivery.Models;
using Delivery.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IDeliveryService, DeliveryService>();
// NOTE: Be aware of this, could contain sensitive data!
builder.Services.AddGrpc(o => o.EnableDetailedErrors = true);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("sqlite")!;
builder.Services.AddDbContext<DeliveryDbContext>(opt =>
    opt.UseSqlite(connectionString: connectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<DeliveryRpcService>();

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
