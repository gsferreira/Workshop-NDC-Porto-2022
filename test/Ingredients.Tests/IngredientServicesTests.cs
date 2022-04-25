using Ingredients.Protos;
using Xunit;

namespace Ingredients.Tests;

public class IngredientServicesTests : IClassFixture<IngredientsApplicationFactory>
{
    private readonly IngredientsApplicationFactory _factory;

    public IngredientServicesTests(IngredientsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async void GetsToppings()
    {
        var clients = _factory.CreateGrpcChannel();
        var response = await clients.GetToppingsAsync(new GetToppingsRequest());
        Assert.NotEmpty(response.Toppings);
        Assert.Collection(response.Toppings, topping => { Assert.Equal("cheese", topping.Id); },
            topping => { Assert.Equal("tomato", topping.Id); });
    }
}