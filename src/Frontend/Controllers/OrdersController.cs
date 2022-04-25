using Frontend.Models;
using Frontend.Orders.Protos;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers;

[Route("orders")]
public class OrdersController : Controller
{
    private readonly ILogger<OrdersController> _log;
    private readonly OrderService.OrderServiceClient _client;
    public OrdersController(ILogger<OrdersController> log, OrderService.OrderServiceClient client)
    {
        _log = log;
        _client = client;
    }

    [HttpPost]
    public async Task<IActionResult> Order([FromForm]HomeViewModel viewModel)
    {
        var request = new PlaceOrderRequest()
        {
            ToppingIds = { viewModel.Toppings.Where(t => t.Selected).Select(t => t.Id) },
            CrustId = viewModel.SelectedCrust
        };

        var response = await _client.PlaceOrderAsync(request);
        
        return View();
    }
}