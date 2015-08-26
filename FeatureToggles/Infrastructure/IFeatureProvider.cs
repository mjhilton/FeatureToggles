namespace FeatureToggles.Infrastructure
{
    public interface IFeatureProvider
    {
        IFeatureToggle Get(string featureName);
    }
}
