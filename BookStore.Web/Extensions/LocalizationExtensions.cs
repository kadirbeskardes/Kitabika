using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace BookStore.Web.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("tr-TR"),
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture("tr-TR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
                options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
                options.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());
            });

            return services;
        }
    }
}