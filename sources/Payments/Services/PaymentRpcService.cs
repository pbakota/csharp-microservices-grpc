namespace Payments.Services;

using Common.Protobuf;

using Payments.Models;

using Payment = Models.Payment;

public class PaymentRpcService : Payments.PaymentsBase
{
    private readonly ILogger<PaymentRpcService> _logger;
    private readonly PaymentDbContext _dbContext;
    private readonly Stocks.StocksClient _stockClient;

    public PaymentRpcService(ILogger<PaymentRpcService> logger, PaymentDbContext dbContext, Stocks.StocksClient stockClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _stockClient = stockClient;
    }

    public override async Task<NewOrderResponse> NewOrder(NewOrderRequest request, Grpc.Core.ServerCallContext context)
    {
        _logger.LogInformation("Received: {}", request);

        var order = request.Order;

        var payment = new Payment
        {
            Amount = order.Amount,
            Mode = order.PaymentMethod,
            OrderId = order.OrderId,
            Status = "Success",
        };

        try
        {
            _dbContext.Add(payment);
            await _dbContext.SaveChangesAsync();

            var paymentRequest = new NewPaymentRequest
            {
                Order = order,
            };

            var result = await _stockClient.NewPaymentAsync(paymentRequest);
            if (!result.Success)
            {
                _logger.LogError("Error when creating paymeny: {} -> {}", order, result.Error);

                payment.Status = "Failed";
                await _dbContext.SaveChangesAsync();

                throw new Exception(result.Error);
            }

            _logger.LogInformation("Payment created: {}", order);

            return new NewOrderResponse
            {
                Success = true,
            };
        }
        catch (Exception)
        {
            payment.Status = "Failed";
            await _dbContext.SaveChangesAsync();

            _logger.LogError("Payment failed: {}", order);

            throw;
        }
    }
}