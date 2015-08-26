using System;
using FeatureToggles.Infrastructure;

namespace FeatureToggles
{
    public static class Features
    {
        private static readonly object InitialisationLock = new object();
        private static bool _initialised;
        
        private static bool _defaultValue;
        private static IFeatureProvider _featureProvider;

        public static void Initialise(IFeatureProvider provider, bool defaultToggleValue = false)
        {
            lock (InitialisationLock)
            {
                _featureProvider = provider;
                _defaultValue = defaultToggleValue;
                _initialised = true;
            }
        }

        public static bool IsEnabled<T>()
            where T : FeatureToggle
        {
            if (!_initialised) throw new InvalidOperationException("Features not initialised. Make sure you've configured a feature provider during app startup.");

            var feature = _featureProvider.Get(typeof (T).Name);

            return (feature != null)
                ? feature.Enabled
                : _defaultValue;
        }
    }
}
