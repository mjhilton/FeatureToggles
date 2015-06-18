namespace FeatureToggles
{
    public class FeatureToggle : IFeatureToggle
    {
        public FeatureToggle(string name, bool enabled)
        {
            FeatureName = name;
            Enabled = enabled;
        }

        public string FeatureName { get; private set; }
        public bool Enabled { get; private set; }
    }
}