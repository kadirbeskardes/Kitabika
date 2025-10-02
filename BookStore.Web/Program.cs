var builder = WebApplication.CreateBuilder(args);

// En minimal logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Sadece temel servisleri ekliyoruz
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Minimal middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Basit bir health check endpoint
app.MapGet("/health", () => "OK");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();