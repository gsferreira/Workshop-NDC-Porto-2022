using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Orders;
using Orders.Ingredients.Protos;
using Orders.Protos;

namespace Orders.Services;

public class OrderServiceImpl : OrderService.OrderServiceBase
{
    private readonly ILogger<OrderServiceImpl> _logger;
    private readonly ingredientsService.ingredientsServiceClient _ingredients;

    public OrderServiceImpl(ILogger<OrderServiceImpl> logger, ingredientsService.ingredientsServiceClient ingredients)
    {
        _logger = logger;
        _ingredients = ingredients;
    }

    public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        var decrementToppingsRequest = new DecrementToppingsRequest()
        {
            ToppingIds = { request.ToppingIds }
        };

        var decrementCrustsRequest = new DecrementCrustsRequest()
        {
            CrustId = request.CrustId
        };

        await Task.WhenAll(
            _ingredients.DecrementToppingsAsync(decrementToppingsRequest).ResponseAsync,
            _ingredients.DecrementCrustsAsync(decrementCrustsRequest).ResponseAsync);

        return new PlaceOrderResponse()
        {
            Time = DateTimeOffset.UtcNow.AddMinutes(30).ToTimestamp()
        };
    }
}