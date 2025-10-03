using Microsoft.Extensions.Localization;

namespace BookStore.Web.Resources
{
    public class SharedResource
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SharedResource(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string GetString(string key)
        {
            return _localizer[key];
        }

        public string GetString(string key, params object[] arguments)
        {
            return _localizer[key, arguments];
        }
    }
}