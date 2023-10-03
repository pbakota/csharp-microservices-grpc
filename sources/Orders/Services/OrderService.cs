
using Common.Protobuf;

using CommonData.Helpers;

using Microsoft.EntityFrameworkCore;

using Orders.Models;

using CustomerOrder = Orders.Models.CustomerOrder;

namespace Orders.Services;

public interface IOrdersService
{
    Task<Order> CreateOrderAsync(CustomerOrder customerOrder);
    Task<List<Order>> GetOrdersAsync(Pagination pagination);
}

public class OrdersService : IOrdersService
{
    private readonly ILogger<OrdersService> _logger;
    private readonly Payments.PaymentsClient _paymentClient;
    private readonly OrderDbContext _dbContext;

    public OrdersService(ILogger<OrdersService> logger,
        OrderDbContext context,
        Payments.PaymentsClient paymentClient)
    {
        _logger = logger;
        _dbContext = context;
        _paymentClient = paymentClient;
    }

    public async Task<Order> CreateOrderAsync(CustomerOrder customerOrder)
    {
        var order = new Order
        {
            Item = customerOrder.Item,
            Amount = customerOrder.Amount,
            Quantity = customerOrder.Quantity,
            Status = "Created",
        };

        try
        {
            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            customerOrder.OrderId = order.Id;

            var orderRequest = new NewOrderRequest
            {
                Order = new Common.Protobuf.CustomerOrder
                {
                    Address = customerOrder.Address,
                    Amount = customerOrder.Amount,
                    Item = customerOrder.Item,
                    OrderId = order.Id,
                    PaymentMethod = customerOrder.PaymentMethod,
                    Quantity = customerOrder.Quantity,
                },
            };

            var result = await _paymentClient.NewOrderAsync(orderRequest);
            if (!result.Success)
            {
                _logger.LogError("Error when creating paymeny: {} -> {}", customerOrder, result.Error);

                order.Status = "Failed";
                await _dbContext.SaveChangesAsync();

                throw new Exception(result.Error);
            }

            _logger.LogInformation("Order created {}", order);
            return order;
        }
        catch (Exception)
        {
            order.Status = "Failed";
            await _dbContext.SaveChangesAsync();

            _logger.LogError("Order not created {}", order);
            throw;
        }
    }

    public async Task<List<Order>> GetOrdersAsync(Pagination pagination)
        => await _dbContext.Orders.OrderByDescending(x => x.Id).Skip(pagination.Skip).Take(pagination.Top).ToListAsync();
}