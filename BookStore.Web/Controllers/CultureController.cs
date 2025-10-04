using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace BookStore.Web.Controllers
{
    public class CultureController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}