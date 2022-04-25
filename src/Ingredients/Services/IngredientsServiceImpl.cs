using Grpc.Core;
using Ingredients.Data;
using Ingredients.Protos;

namespace Ingredients.Services;

internal class IngredientsServiceImpl : ingredientsService.ingredientsServiceBase
{
    private readonly IToppingData _toppingData;
    private readonly ICrustData _crustData;
    private readonly ILogger<IngredientsServiceImpl> _logger;

    public IngredientsServiceImpl(ILogger<IngredientsServiceImpl> logger, IToppingData toppingData, ICrustData crustData)
    {
        _logger = logger;
        _toppingData = toppingData;
        _crustData = crustData;
    }

    public override async Task<GetToppingsResponse> GetToppings(GetToppingsRequest request, ServerCallContext context)
    {
        try
        {
            var toppings = await _toppingData.GetAsync(context.CancellationToken);

            var response = new GetToppingsResponse()
            {
                Toppings =
                {
                    toppings.Select(t => new Topping()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Price = t.Price
                    })
                }
            };

            return response;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw new RpcException(new Status(StatusCode.Internal, exception.Message, exception));
        }
    }
    
    public override async Task<GetCrustsResponse> GetCrusts(GetCrustsRequest request, ServerCallContext context)
    {
        try
        {
            var data = await _crustData.GetAsync(context.CancellationToken);

            var response = new GetCrustsResponse()
            {
                Crusts =
                {
                    data.Select(t => new Crust()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Price = t.Price,
                        Size = t.Size,
                        StockCount = t.StockCount
                    })
                }
            };

            return response;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw new RpcException(new Status(StatusCode.Internal, exception.Message, exception));
        }
    }

    
}