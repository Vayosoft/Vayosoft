using System.Reflection;
using Microsoft.Extensions.Localization;
using Vayosoft.Web.Resources;

namespace Vayosoft.Web.Localization
{
    public class SharedLocalizationService
    {
        private readonly IStringLocalizer _localizer;
        public SharedLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResources).GetTypeInfo().Assembly.FullName!);
            _localizer = factory.Create(nameof(SharedResources), assemblyName.Name!);
        }

        public string Get(string key)
        {
            return _localizer[key];
        }

        public string this[string key] => _localizer[key];
    }
}
