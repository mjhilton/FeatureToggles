using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles.Infrastructure
{
    public class PollingFeatureCache : IFeatures
    {
        private readonly object _cacheLock = new object();
        private readonly IFeaturesContext _featuresContext;

        private Dictionary<string, FeatureToggle> _featureToggleCache;

        public PollingFeatureCache(IFeaturesContext featuresContext)
            : this(featuresContext, TimeSpan.FromMinutes(10))
        {
        }

        public PollingFeatureCache(IFeaturesContext featuresContext, TimeSpan updateInterval)
        {
            _featuresContext = featuresContext;
            _featureToggleCache = new Dictionary<string, FeatureToggle>();

            Task.WaitAll(new [] { UpdateFromSource() });
            ScheduleRecurringUpdate(updateInterval);
        }

        public IFeatureToggle Get(string featureName)
        {
            lock (_cacheLock)
            {
                return _featureToggleCache.ContainsKey(featureName)
                    ? _featureToggleCache[featureName]
                    : null;
            }
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
            var features = await _featuresContext.Features.ToDictionaryAsync(
                    f => f.Name,
                    f => new FeatureToggle(f.Name, f.Enabled));

            lock (_cacheLock)
            {
                _featureToggleCache.Clear();
                _featureToggleCache = features;
            }
        }
    }
}
