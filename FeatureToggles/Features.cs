using System;
using System.Collections.Generic;
using FeatureToggles.Infrastructure;

namespace FeatureToggles
{
    public static class Features
    {
        private static readonly object InitialisationLock = new object();
        private static bool _initialised;
        
        private static bool _defaultValue;
        private static IList<IFeatureProvider> _featureProviders;

        internal static void Initialise(IFeatureProvider provider, bool defaultToggleValue = false)
        {
            Initialise(new [] { provider }, defaultToggleValue);
        }

        internal static void Initialise(IList<IFeatureProvider> providers, bool defaultToggleValue = false)
        {
            lock (InitialisationLock)
            {
                _featureProviders = providers;
                _defaultValue = defaultToggleValue;
                _initialised = true;
            }
        }

        public static bool IsEnabled<T>()
            where T : FeatureToggle
        {
            if (!_initialised) throw new InvalidOperationException("Features not initialised. Make sure you've configured a feature provider during app startup.");

            foreach (var provider in _featureProviders)
            {
                var feature = provider.Get(typeof(T).Name);
                
                if (feature == null) 
                    continue;
                
                return feature.Enabled;
            }

            return _defaultValue;
        }
    }
}
