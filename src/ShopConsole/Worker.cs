using Grpc.Core;
using Orders.Protos;

namespace ShopConsole;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OrderService.OrderServiceClient _client;

    public Worker(ILogger<Worker> logger, OrderService.OrderServiceClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var call = _client.Subscribe(new SubscribeRequest(), cancellationToken: stoppingToken);
                await foreach (var notification in call.ResponseStream.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine($"order receive: {notification.CrustId}");
                    foreach (var topping in notification.ToppingIds)
                    {
                        Console.WriteLine($"     topping: {topping}");
                    }

                    Console.WriteLine($"Due by: {notification.Time.ToDateTimeOffset():t}");
                }
            }
            catch (OperationCanceledException)
            {
                if(stoppingToken.IsCancellationRequested)
                    break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
                throw;
            }
        }
    }
}
