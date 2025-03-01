﻿using System.Diagnostics;
using Frontend.Ingredients.Protos;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;

namespace Frontend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ingredientsService.ingredientsServiceClient _ingredients;
    public HomeController( ILogger<HomeController> logger, ingredientsService.ingredientsServiceClient ingredients)
    {
        _logger = logger;
        _ingredients = ingredients;
    }

    public async Task<IActionResult> Index()
    {
        var toppings = await GetToppingsAsync();
        var crusts = await GetCrustsAsync();

        var viewModel = new HomeViewModel(toppings, crusts);
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<List<ToppingViewModel>> GetToppingsAsync()
    {
        var toppingResponse = await _ingredients.GetToppingsAsync(new GetToppingsRequest());

        var toppings = toppingResponse.Toppings
            .Select(t => new ToppingViewModel(t.Id, t.Name, Convert.ToDecimal(t.Price)))
            .ToList();

        return toppings;
    }
    
    private async Task<List<CrustViewModel>> GetCrustsAsync()
    {
        var crustsResponse = await _ingredients.GetCrustsAsync(new GetCrustsRequest());

        var crusts = crustsResponse.Crusts
            .Select(t => new CrustViewModel(t.Id, t.Name, t.Size, Convert.ToDecimal(t.Price)))
            .ToList();

        return crusts;
    }
}