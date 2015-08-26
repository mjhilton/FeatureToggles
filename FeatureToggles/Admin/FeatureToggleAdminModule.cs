using Autofac;

namespace FeatureToggles.Admin
{
    public class FeatureToggleAdminModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<FeatureManager>().As<IFeatureManager>();
        }
    }
}
