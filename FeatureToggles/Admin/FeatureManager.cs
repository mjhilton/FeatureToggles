using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FeatureToggles.Infrastructure;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles.Admin
{
    class FeatureManager : IFeatureManager
    {
        private readonly IFeaturesContext _context;

        public FeatureManager(FeatureManagerConfiguration config)
        {
            _context = new FeaturesContext(config.NameOrConnectionString);
        }

        public IEnumerable<FeatureSchedule> GetAll()
        {
            var features = _context.Features
                .GroupBy(f => f.Name)
                .ToList();

            foreach (var feature in features)
            {
                var scheduledValues = feature.OrderBy(f => f.EffectiveAt);

                var pastValues = scheduledValues.TakeWhile(f => f.EffectiveAt < Clock.Now).ToList();
                var currentValue = pastValues.LastOrDefault();
                var futureValues = scheduledValues.Except(pastValues);
                
                yield return new FeatureSchedule
                {
                    Name = feature.Key,
                    CurrentValue = new KeyValuePair<DateTimeOffset, bool>(currentValue.EffectiveAt, currentValue.Enabled),
                    ScheduledValues = futureValues.Select(f => new KeyValuePair<DateTimeOffset, bool>(f.EffectiveAt, f.Enabled)).ToList()
                };
            }
        }

        public TryResult Add(string name, bool defaultValue)
        {
            if (_context.Features.Any(f => f.Name == name))
                return TryResult.Failed(string.Format("Feature '{0}' already exists.", name));

            _context.Features.Add(new Feature(name, defaultValue));
            _context.Save();
            return TryResult.Succeeded();
        }

        public TryResult Update(string name, bool newValue)
        {
            var currentValue = CurrentValue(f => f.Name == name);
            
            if (currentValue == null) 
                return TryResult.Failed(string.Format("Feature '{0}' not found.", name));

            return Schedule(name, Clock.Now, newValue);
        }

        public TryResult Schedule(string name, DateTimeOffset effectiveTime, bool newValue)
        {
            var existing = _context.Features.Where(f => f.Name == name && f.EffectiveAt == effectiveTime);

            if (existing.Any())
            {
                foreach (var scheduledValue in existing)
                    scheduledValue.Enabled = newValue;
            }
            else
            {
                _context.Features.Add(new Feature(name, newValue) { EffectiveAt = effectiveTime });
            }

            _context.Save();
            return TryResult.Succeeded();
        }

        public TryResult Deschedule(string name, DateTimeOffset effectiveTime)
        {
            var existing = _context.Features.Where(f => f.Name == name && f.EffectiveAt == effectiveTime);

            if (!existing.Any())
                return TryResult.Failed(string.Format("Feature '{0}' has no scheduled value for '{1}'", name, effectiveTime));

            foreach (var scheduledValue in existing)
                _context.Features.Remove(scheduledValue);

            _context.Save();
            return TryResult.Succeeded();
        }

        public TryResult Remove(string name)
        {
            var existing = _context.Features.Where(f => f.Name == name);

            if (!existing.Any())
                return TryResult.Failed(string.Format("Feature '{0}' not found.", name));

            foreach (var scheduledValue in existing)
                _context.Features.Remove(scheduledValue);

            _context.Save();
            return TryResult.Succeeded();
        }

        private Feature CurrentValue(Expression<Func<Feature, bool>> predicate = null)
        {
            predicate = predicate ?? (f => true);
            return PastValues(predicate).OrderByDescending(f => f.EffectiveAt).FirstOrDefault();
        }

        private IEnumerable<Feature> PastValues(Expression<Func<Feature, bool>> predicate = null)
        {
            predicate = predicate ?? (f => true);

            return _context.Features
                .Where(predicate)
                .Where(f => f.EffectiveAt <= Clock.Now);
        }

        private IEnumerable<Feature> FutureValues(Expression<Func<Feature, bool>> predicate = null)
        {
            predicate = predicate ?? (f => true);

            return _context.Features
                .Where(predicate)
                .Where(f => f.EffectiveAt > Clock.Now);
        }
    }
}