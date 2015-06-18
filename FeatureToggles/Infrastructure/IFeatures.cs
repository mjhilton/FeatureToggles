namespace FeatureToggles.Infrastructure
{
    public interface IFeatures
    {
        IFeatureToggle Get(string featureName);
    }
}
