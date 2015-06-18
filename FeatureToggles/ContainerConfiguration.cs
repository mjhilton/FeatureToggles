using Autofac;
using FeatureToggles.Infrastructure;
using FeatureToggles.Infrastructure.EntityFramework;

namespace FeatureToggles
{
    public class ContainerConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<FeaturesContext>()
                .AsImplementedInterfaces();

            builder.RegisterType<PollingFeatureCache>()
                .As<IFeatures>()
                .SingleInstance();
        }
    }
}
