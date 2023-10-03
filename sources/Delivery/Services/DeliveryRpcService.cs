using Common.Protobuf;

using Delivery.Models;

using Grpc.Core;

namespace Delivery.Services;

public class DeliveryRpcService : Deliveries.DeliveriesBase
{
    private readonly ILogger<DeliveryRpcService> _logger;
    private readonly DeliveryDbContext _dbContext;

    public DeliveryRpcService(ILogger<DeliveryRpcService> logger,
        DeliveryDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public override async Task<NewDeliveryResponse> NewDelivery(NewDeliveryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received: {}", request);

        var order = request.Order;

        var shipment = new DeliveryItem
        {
            Address = order.Address,
            OrderId = order.OrderId,
            Status = "Success"
        };

        // NOTE: Here we could use transaction, however if there is an error, we will not store any data to the DB, and we will not be able to
        // track the whole workflow.
        try
        {
            if (string.IsNullOrEmpty(order.Address))
            {
                throw new Exception("Address not present");
            }

            _dbContext.Add(shipment);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Delivered to {}", shipment);

            return new NewDeliveryResponse
            {
                Success = true,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured in delivery");

            shipment.Status = "Failed";
            _dbContext.Add(shipment);
            await _dbContext.SaveChangesAsync();

            return new NewDeliveryResponse
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
}