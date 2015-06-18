using System.Data.Entity;

namespace FeatureToggles.Infrastructure.EntityFramework
{
    public interface IFeaturesContext
    {
        IDbSet<Feature> Features { get; }
    }
}