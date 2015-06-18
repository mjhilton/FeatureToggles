namespace FeatureToggles
{
    public interface IFeatureToggle
    {
        string FeatureName { get; }
        bool Enabled { get; }
    }
}
