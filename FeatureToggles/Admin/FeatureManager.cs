using System;
using System.Collections.Generic;
using System.Linq;
using FeatureToggles.Infrastructure;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles.Admin
{
    class FeatureManager : IFeatureManager
    {
        private readonly IFeaturesContext _context;

        public FeatureManager(IFeaturesContext context)
        {
            _context = context;
        }

        public IEnumerable<FeatureSchedule> GetAll()
        {
            var features = _context.Features
                .OrderBy(f => f.EffectiveAt)
                .GroupBy(f => f.Name)
                .ToList();

            foreach (var feature in features)
            {
                var pastValues = feature.TakeWhile(f => f.EffectiveAt.UtcDateTime < Clock.UtcNow).ToList();
                var currentValue = pastValues.Last();
                var futureValues = feature.Except(pastValues);
                
                yield return new FeatureSchedule
                {
                    Name = feature.Key,
                    CurrentValue = new KeyValuePair<DateTimeOffset, bool>(currentValue.EffectiveAt, currentValue.Enabled),
                    ScheduledValues = futureValues.Select(f => new KeyValuePair<DateTimeOffset, bool>(f.EffectiveAt, f.Enabled)).ToList()
                };
            }
        }

        public void Add(string name, bool defaultValue)
        {
            if (_context.Features.Any(f => f.Name == name)) throw new InvalidOperationException(string.Format("Feature '{0}' already exists.", name));

            _context.Features.Add(new Feature(name, defaultValue));
        }

        public void Update(string name, bool newValue)
        {
            var existing = _context.Features.Where(f => f.Name == name);
            
            if (existing.Count() > 1) throw new InvalidOperationException(string.Format("Feature '{0}' has existing scheduled values. Remove these first.", name));

            existing.Single().Enabled = newValue;
        }

        public void Schedule(string name, DateTimeOffset effectiveTime, bool newValue)
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
        }

        public void Deschedule(string name, DateTimeOffset effectiveTime)
        {
            var existing = _context.Features.Where(f => f.Name == name && f.EffectiveAt == effectiveTime);

            if (!existing.Any()) throw new InvalidOperationException(string.Format("Feature '{0}' has no scheduled value for '{1}'", name, effectiveTime));

            foreach (var scheduledValue in existing)
                _context.Features.Remove(scheduledValue);
        }

        public void Remove(string name)
        {
            var existing = _context.Features.Where(f => f.Name == name);

            if (!existing.Any()) throw new InvalidOperationException(string.Format("Feature '{0}' not found.", name));

            foreach (var scheduledValue in existing)
                _context.Features.Remove(scheduledValue);
        }
    }
}