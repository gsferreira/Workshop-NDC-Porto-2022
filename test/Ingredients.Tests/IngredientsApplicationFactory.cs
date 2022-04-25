using System.Collections.Generic;
using System.Threading;
using Grpc.Net.Client.Balancer;
using Ingredients.Data;
using Ingredients.Protos;
using Ingredients.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Ingredients.Tests;

public class IngredientsApplicationFactory: WebApplicationFactory<IAssemblyMarker>
{
    public ingredientsService.ingredientsServiceClient CreateGrpcChannel()
    {
        var channel = GrpcTestHelper.WebApplicationFactoryExtensions.CreateGrpcChannel(this);
        return new ingredientsService.ingredientsServiceClient(channel);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IToppingData>();

            var toppings = new List<ToppingEntity>()
            {
                new ToppingEntity("cheese", "cheese", 0.5d, 10),
                new ToppingEntity("tomato", "tomato", 0.5d, 100)
            };
            var toppingSub = Substitute.For<IToppingData>();
            toppingSub.GetAsync(Arg.Any<CancellationToken>())
                .Returns(toppings);
            services.AddSingleton(toppingSub);
            
            
            services.RemoveAll<ICrustData>();

            var crusts = new List<CrustEntity>()
            {
                new CrustEntity("thin9", "Thin",9, 5d, 10),
                new CrustEntity("deep9", "Deep", 9, 6d, 100)
            };
            var crustSub = Substitute.For<ICrustData>();
            crustSub.GetAsync(Arg.Any<CancellationToken>())
                .Returns(crusts);
            services.AddSingleton(crustSub);      

        });
        base.ConfigureWebHost(builder);
    }
}