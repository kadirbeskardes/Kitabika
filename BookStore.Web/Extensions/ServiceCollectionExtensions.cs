using BookStore.Core.Interfaces;
using BookStore.Data;
using BookStore.Data.Repositories;
using BookStore.Service.Interfaces;
using BookStore.Service.Services;
using BookStore.Web.Helpers;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BookStore.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBookStoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                            ?? configuration.GetConnectionString("SQL_CONNECTION_STRING");
        
        services.AddDbContext<BookStoreContext>(options =>
            options.UseSqlServer(connectionString));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Culture settings
        var cultureInfo = new CultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        // AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));

        // Business Services
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IFavoriteService, FavoriteService>();

        // Repositories
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();

        // Background Services
        services.AddHostedService<OverdueLoanCheckerService>();

        // Authentication
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

        // Session
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        return services;
    }
}