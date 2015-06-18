using System.Data.Entity;

namespace FeatureToggles.Infrastructure.EntityFramework
{
    public class FeaturesContext : DbContext, IFeaturesContext
    {
        public FeaturesContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public IDbSet<Feature> Features { get; set; }
    }

    public interface IFeaturesContext
    {
        IDbSet<Feature> Features { get; }
    }
}
