using System;

namespace FeatureToggles.Infrastructure
{
    internal class Feature
    {
        public Feature() { }

        public Feature(string name, bool initialValue)
        {
            Id = Guid.NewGuid();
            Name = name;
            Enabled = initialValue;
            EffectiveAt = Clock.Now;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public DateTimeOffset EffectiveAt { get; set; }
    }
}
