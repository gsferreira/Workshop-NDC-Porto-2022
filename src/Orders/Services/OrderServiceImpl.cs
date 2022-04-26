using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Orders;
using Orders.Ingredients.Protos;
using Orders.Protos;
using Orders.PubSub;

namespace Orders.Services;

public class OrderServiceImpl : OrderService.OrderServiceBase
{
    private readonly ILogger<OrderServiceImpl> _logger;
    private readonly ingredientsService.ingredientsServiceClient _ingredients;
    private readonly IOrderPublisher _publisher;
    private readonly IOrderMessages _messages;

    public OrderServiceImpl(ILogger<OrderServiceImpl> logger, ingredientsService.ingredientsServiceClient ingredients,
        IOrderPublisher publisher, IOrderMessages messages)
    {
        _logger = logger;
        _ingredients = ingredients;
        _publisher = publisher;
        _messages = messages;
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

        var time = DateTimeOffset.UtcNow.AddMinutes(30);
        await _publisher.PublishOrder(request.CrustId, request.ToppingIds, time);

        await Task.WhenAll(
            _ingredients.DecrementToppingsAsync(decrementToppingsRequest).ResponseAsync,
            _ingredients.DecrementCrustsAsync(decrementCrustsRequest).ResponseAsync);

        return new PlaceOrderResponse()
        {
            Time = time.ToTimestamp()
        };
    }

    public override async Task Subscribe(SubscribeRequest request,
        IServerStreamWriter<OrderNotification> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                var message = await _messages.ReadAsync(context.CancellationToken);
                var notification = new OrderNotification()
                {
                    CrustId = message.CrustId,
                    Time = message.Time.ToTimestamp(),
                    ToppingIds = { message.ToppingIds }
                };
                await responseStream.WriteAsync(notification);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

    }
}