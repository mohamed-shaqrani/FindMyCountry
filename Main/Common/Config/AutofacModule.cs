using Autofac;
using FluentValidation;
using Main.Common.Base;
using Main.Common.Repository;
using Main.Feature.IpGeoLocation.Endpoints.Countries;
using Main.Feature.IpGeoLocation.Queries;
using Main.Repository;
using MediatR;

namespace Main.Common.Config;
internal sealed class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IpGeolocationHandler).Assembly);
        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();
        //register ICountryIPTracker 
        builder.RegisterType<ICountryIPTracker>().AsImplementedInterfaces().InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(IValidator<>))
                .InstancePerLifetimeScope();
        builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();
        builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
        builder.RegisterType<Mediator>().As<IMediator>();
        builder.RegisterGeneric(typeof(BaseEndpointParam<>))
            .AsSelf()
            .InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(typeof(GetUsersHandler).Assembly);
    }
}