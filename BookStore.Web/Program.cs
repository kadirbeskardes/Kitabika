using BookStore.Service.Services;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BookStore.Web.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using BookStore.Core.Interfaces;
using BookStore.Data;
using System.Globalization;
using BookStore.Data.Repositories;
using BookStore.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging'i basit tutuyoruz
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Connection string debug - her iki ismi de deneyelim
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var sqlConnectionString = builder.Configuration.GetConnectionString("SQL_CONNECTION_STRING");

Console.WriteLine($"🔍 DefaultConnection Found: {!string.IsNullOrEmpty(connectionString)}");
Console.WriteLine($"🔍 SQL_CONNECTION_STRING Found: {!string.IsNullOrEmpty(sqlConnectionString)}");

// Hangisi varsa onu kullan
var finalConnectionString = connectionString ?? sqlConnectionString;

Console.WriteLine($"🔍 Final Connection String Found: {!string.IsNullOrEmpty(finalConnectionString)}");
if (!string.IsNullOrEmpty(finalConnectionString))
{
    // Sadece server kısmını göster (güvenlik için)
    var serverPart = finalConnectionString.Split(';')[0];
    Console.WriteLine($"🔍 Server: {serverPart}");
}

builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlServer(finalConnectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var cultureInfo = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ILoanService, LoanService>();

builder.Services.AddHostedService<OverdueLoanCheckerService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Detaylı hata yakalama
try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application starting - services built successfully");
    
    // Database connection test (migration olmadan)
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BookStoreContext>();
            var canConnect = context.Database.CanConnect();
            logger.LogInformation($"Database connection test: {(canConnect ? "SUCCESS" : "FAILED")}");
        }
    }
    catch (Exception dbEx)
    {
        var logger2 = app.Services.GetRequiredService<ILogger<Program>>();
        logger2.LogError(dbEx, "Database connection failed, but application will continue");
    }
}
catch (Exception ex)
{
    // Bu durumda logger bile çalışmayabilir, console'a yazdır
    Console.WriteLine($"CRITICAL ERROR during app initialization: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Development'ta detaylı hata göster (Azure'da da development mode'da çalıştırmak için)
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();