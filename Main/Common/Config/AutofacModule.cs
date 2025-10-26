using Autofac;
using FluentValidation;
using Main.Common.Base;
using Main.Feature.IpGeoLocation.Endpoints.Countries;
using Main.Feature.IpGeoLocation.Endpoints.Logs;
using Main.Feature.IpGeoLocation.Queries;

namespace Main.Common.Config;
internal sealed class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {


        builder.RegisterGeneric(typeof(BaseEndpointParam<>))
                .AsSelf()
                .InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(typeof(IpGeolocationHandler).Assembly);

        builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(IValidator<>))
                .InstancePerLifetimeScope();
        builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();
        builder.RegisterType<BlockedCountryIPTracker>()
                .As<ICountryIPTracker>()
                .SingleInstance();
        builder.RegisterType<BlockedAttemptMemoryStore>()
             .As<IBlockedAttemptStore>()
             .SingleInstance();

    }
}