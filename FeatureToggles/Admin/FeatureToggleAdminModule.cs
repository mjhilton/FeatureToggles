using Autofac;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles.Admin
{
    public class FeatureToggleAdminModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<FeaturesContext>().As<IFeaturesContext>();
            builder.RegisterType<FeatureManager>().As<IFeatureManager>();
        }
    }
}
