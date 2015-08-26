using System.Collections.Generic;
using System.Linq;
using FeatureToggles.Infrastructure;

namespace FeatureToggles.Mvc
{
    public static class FeatureConfig
    {
        public static void UseProviders(IEnumerable<IFeatureProvider> providers, bool defaultValue = false)
        {
            Features.Initialise(providers.ToList(), defaultValue);
        }
    }
}
