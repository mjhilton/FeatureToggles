using FeatureToggles.Infrastructure;

namespace FeatureToggles.Mvc
{
    public static class FeatureConfig
    {
        public static void UsePollingSqlProvider(string nameOrConnectionString, bool defaultToggleValue = false)
        {
            Features.Initialise(new PollingCacheFeatureProvider(nameOrConnectionString), defaultToggleValue);
        }
    }
}
