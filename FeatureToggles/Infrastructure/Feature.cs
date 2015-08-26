using System;

namespace FeatureToggles.Infrastructure
{
    internal class Feature
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public DateTimeOffset EffectiveAt { get; set; }
    }
}
