using System;

namespace FeatureToggles.Infrastructure.EntityFramework
{
    public class EntityFrameworkFeatureRepository : IFeatures
    {
        public IFeatureToggle Get(string featureName)
        {
            throw new NotImplementedException();
        }
    }
}
