using System.Runtime.InteropServices;

using Common.Protobuf;

using Microsoft.EntityFrameworkCore;

using Stock.Models;
using Stock.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IStockService, StockService>();
// NOTE: Be aware of this, could contain sensitive data!
builder.Services.AddGrpc(o => o.EnableDetailedErrors = true);

var client = builder.Services.AddGrpcClient<Deliveries.DeliveriesClient>(o => o.Address = new Uri(builder.Configuration.GetSection("Grpc")["DeliveriesAddress"]));

// if (builder.Environment.IsDevelopment() || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
    // NOTE: Normally gRPC should communicate with encripted channel (SSL), however
    // for development (and on Linux) it is easier to not bother with cerificate.
    // On Windows you can just trust the dev certificate and that should be enough.
    client.ConfigurePrimaryHttpMessageHandler(() =>
    {
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        return httpHandler;
    });
// }

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("sqlite")!;
builder.Services.AddDbContext<StockDbContext>(opt =>
    opt.UseSqlite(connectionString: connectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<StockRpcService>();

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
