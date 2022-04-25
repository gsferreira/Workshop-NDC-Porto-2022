using Frontend.Ingredients.Protos;
using Frontend.Orders.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var ingredientsUri = builder.Configuration.GetServiceUri("ingredients") ?? new Uri("https://localhost:7105");
builder.Services.AddGrpcClient<ingredientsService.ingredientsServiceClient>(
    options =>
    {
        options.Address = ingredientsUri;
    });

var ordersUri = builder.Configuration.GetServiceUri("orders") ?? new Uri("https://localhost:7106");
builder.Services.AddGrpcClient<OrderService.OrderServiceClient>(
    options =>
    {
        options.Address = ordersUri;
    });
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
