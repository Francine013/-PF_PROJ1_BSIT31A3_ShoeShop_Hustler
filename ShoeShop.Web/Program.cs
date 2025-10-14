

using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Data;
using ShoeShop.Services.Interfaces;
using ShoeShop.Services.Services; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IDashboardService, DashboardService>();


builder.Services.AddDbContext<ShoeShopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddScoped<ISupplierService, SupplierService>();

builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPullOutService, PullOutService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IReportService, ReportService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ShoeShopDbContext>();

    context.Database.EnsureCreated();
 
}

app.Run();
