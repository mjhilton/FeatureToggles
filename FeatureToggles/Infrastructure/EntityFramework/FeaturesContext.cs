using System.Data.Entity;

namespace FeatureToggles.Infrastructure.EntityFramework
{
    class FeaturesContext : DbContext, IFeaturesContext
    {
        public FeaturesContext(string nameOrConnectionString = "FeaturesContext")
            : base(nameOrConnectionString)
        {
        }

        public IDbSet<Feature> Features { get; set; }
    }
}
