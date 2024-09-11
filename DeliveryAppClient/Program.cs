using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<INavigationService>(_ =>
{
    return new ServiceBusQueue(builder.Configuration.GetConnectionString("ServiceBus")!);
});
builder.Services.AddTransient<IProductService>(_ =>
{
    return new ProductService(builder.Configuration.GetConnectionString("Storage")!);
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromSeconds(60);
    opt.Cookie.IsEssential = true;
    opt.Cookie.HttpOnly = true;
});

//builder.Services.AddTransient<ServiceBusQueue>();

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

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
