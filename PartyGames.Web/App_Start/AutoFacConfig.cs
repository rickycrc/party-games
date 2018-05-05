using System.Data.Entity;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using PartyGames.Data;
using PartyGames.Data.BingoContext;
using PartyGames.Service.Bingo;
using PartyGames.Service.Caching;
using PartyGames.Service.WebService;
using PartyGames.Web.Infrastructure.Layout;

namespace PartyGames.Web
{
    public class AutoFacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // OPTIONAL: Enable action method parameter injection (RARE).
            //builder.InjectActionInvoker();

            //db context
            builder.RegisterType<BingoContext>().As(typeof(DbContext)).InstancePerLifetimeScope();

            //repository
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            

            //services
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().SingleInstance();

            builder.RegisterType<BingoService>().As<IBingoService>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerService>().As<IPlayerService>().InstancePerLifetimeScope();
            builder.RegisterType<MarkCalculationService>().As<IMarkCalculationService>().InstancePerLifetimeScope();
            builder.RegisterType<EposWebService>().As<IEposWebService>().InstancePerLifetimeScope();
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();


            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}