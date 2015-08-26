using System.Data.Entity;

namespace FeatureToggles.Infrastructure.EntityFramework
{
    interface IFeaturesContext
    {
        IDbSet<Feature> Features { get; }
        void Save();
    }
}