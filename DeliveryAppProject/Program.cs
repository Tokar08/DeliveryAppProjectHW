using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddTransient<INavigationService>(_ =>
{
    return new ServiceBusQueue(builder.Configuration.GetConnectionString("ServiceBus")!);
});
builder.Services.AddTransient<IProductService>(_ =>
{
    return new ProductService(builder.Configuration.GetConnectionString("Storage")!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
