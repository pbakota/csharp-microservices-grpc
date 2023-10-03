using System.Runtime.InteropServices;

using Common.Protobuf;

using Microsoft.EntityFrameworkCore;

using Orders.Middlewar;
using Orders.Models;
using Orders.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IOrdersService, OrdersService>();

var client = builder.Services.AddGrpcClient<Payments.PaymentsClient>(o => o.Address = new Uri(builder.Configuration.GetSection("Grpc")["PaymentsAddress"]));

if (builder.Environment.IsDevelopment() || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
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
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("sqlite")!;
builder.Services.AddDbContext<OrderDbContext>(opt =>
    opt.UseSqlite(connectionString: connectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
