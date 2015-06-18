using System;

namespace FeatureToggles.Infrastructure
{
    public class Feature
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public DateTimeOffset EffectiveAt { get; set; }
    }
}
