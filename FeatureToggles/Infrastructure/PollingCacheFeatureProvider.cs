using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles.Infrastructure
{
    class PollingCacheFeatureProvider : IFeatureProvider
    {
        private readonly object _cacheLock = new object();
        private readonly IFeaturesContext _featuresContext;

        private List<Feature> _featureToggleCache;

        public PollingCacheFeatureProvider(string nameOrConnectionString)
            : this (new FeaturesContext(nameOrConnectionString))
        {
        }

        public PollingCacheFeatureProvider(IFeaturesContext featuresContext)
            : this(featuresContext, TimeSpan.FromMinutes(10))
        {
        }

        public PollingCacheFeatureProvider(IFeaturesContext featuresContext, TimeSpan updateInterval)
        {
            _featuresContext = featuresContext;
            _featureToggleCache = new List<Feature>();

            Task.WaitAll(new [] { UpdateFromSource() });
            ScheduleRecurringUpdate(updateInterval);
        }

        public IFeatureToggle Get(string featureName)
        {
            lock (_cacheLock)
            {
                return IsRegistered(featureName)
                    ? GetActiveToggleValue(featureName)
                    : null;
            }
        }

        public bool IsRegistered(string featureName)
        {
            return _featureToggleCache.Any(f => f.Name == featureName);
        }

        private IFeatureToggle GetActiveToggleValue(string featureName)
        {
            var matchingFeatures = _featureToggleCache.Where(f => f.Name == featureName).ToList();
            var effectiveDatePassed = matchingFeatures.Where(f => f.EffectiveAt.UtcDateTime <= Clock.UtcNow).ToList();

            var activeFeature = effectiveDatePassed    
                .OrderByDescending(f => f.EffectiveAt)
                .First();

            return new FeatureToggle(activeFeature.Name, activeFeature.Enabled);
        }

        private void ScheduleRecurringUpdate(TimeSpan interval)
        {
            Task.Delay(interval)
                .ContinueWith(t => UpdateFromSource())
                .ContinueWith(t =>
                {
                    if (t.Status != TaskStatus.RanToCompletion)
                        throw new Exception("Failed to schedule next update");

                    ScheduleRecurringUpdate(interval);
                });
        }

        private async Task UpdateFromSource()
        {
            var features = await _featuresContext.Features.ToListAsync();

            lock (_cacheLock)
            {
                _featureToggleCache.Clear();
                _featureToggleCache = features;
            }
        }
    }
}
