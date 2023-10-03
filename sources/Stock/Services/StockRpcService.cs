using Common.Protobuf;

using Grpc.Core;

using Stock.Models;

namespace Stock.Services;

public class StockRpcService : Stocks.StocksBase
{
    private readonly ILogger<StockRpcService> _logger;
    private readonly StockDbContext _dbContext;
    private readonly Deliveries.DeliveriesClient _deliveryClient;

    public StockRpcService(ILogger<StockRpcService> logger, StockDbContext dbContext, Deliveries.DeliveriesClient deliveryClient)

    {
        _logger = logger;
        _dbContext = dbContext;
        _deliveryClient = deliveryClient;
    }

    public override async Task<NewPaymentResponse> NewPayment(NewPaymentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received: {}", request.Order);

        var order = request.Order;
        var invUpdated = false;

        try
        {
            var inv = _dbContext.StockItems.Where(x => x.Item == order.Item).FirstOrDefault();
            if (inv == default)
            {
                _logger.LogWarning("Stock not exits so reverting the order");
                throw new Exception("Stock not available");
            }

            inv.Quantity += -order.Quantity;
            await _dbContext.SaveChangesAsync();

            invUpdated = true;

            var deliveryRequest = new NewDeliveryRequest
            {
                Order = order,
            };

            var result = await _deliveryClient.NewDeliveryAsync(deliveryRequest);
            if (!result.Success)
            {
                throw new Exception(result.Error);
            }

            _logger.LogInformation("Stock updated: {}", order);

            return new NewPaymentResponse
            {
                Success = true,
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Stock change failed: {}", order);
            // NOTE: This is a tricky part. Since we do not know for sure did we decreased the stock quantity
            // or if the exception happened before that, to distinguish between these two states we will use "invUpdated" flag.
            if (invUpdated)
            {
                var inv = _dbContext.StockItems.Where(x => x.Item == order.Item).FirstOrDefault();
                if (inv != default)
                {
                    inv.Quantity += order.Quantity;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return new NewPaymentResponse
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
}
