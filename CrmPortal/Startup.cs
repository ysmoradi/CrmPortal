using Bit.Core;
using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Bit.Core.Models;
using Bit.Hangfire.Implementations;
using Bit.Owin;
using Bit.Owin.Implementations;
using CrmPortal.Api.Contracts;
using CrmPortal.Api.Implementations;
using CrmPortal.Data;
using CrmPortal.Data.Contracts;
using CrmPortal.Data.Implementations;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CrmPortal
{
    public class Startup : AutofacAspNetCoreAppStartup
    {
        public Startup(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DefaultAppModulesProvider.Current = new CrmPortalDependencies();

            return base.ConfigureServices(services);
        }
    }

    public class CrmPortalDependencies : IAppModulesProvider, IAppModule
    {
        public IEnumerable<IAppModule> GetAppModules()
        {
            yield return this;
        }

        public virtual void ConfigureDependencies(IServiceCollection services, IDependencyManager dependencyManager)
        {
            #region Initial Configuration

            AssemblyContainer.Current.Init();
            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(typeof(DebugLogStore).GetTypeInfo(), typeof(ConsoleLogStore).GetTypeInfo());

            dependencyManager.RegisterDefaultAspNetCoreApp();
            dependencyManager.RegisterMinimalAspNetCoreMiddlewares();

            #endregion

            AppEnvironment appEnv = DefaultAppEnvironmentsProvider.Current.GetActiveAppEnvironment();

            #region Bit Identity Client

            dependencyManager.RegisterAspNetCoreSingleSignOnClient();

            #endregion

            #region Bit Web Api

            dependencyManager.RegisterDefaultWebApiConfiguration();

            dependencyManager.RegisterWebApiMiddleware(webApiDependencyManager =>
            {
                webApiDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
                });

                webApiDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.EnableSwagger(c =>
                    {
                        c.SingleApiVersion($"v{appEnv.AppInfo.Version}", $"{appEnv.AppInfo.Name}-Api");
                        c.ApplyDefaultApiConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });

                webApiDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();
            });

            #endregion

            #region Bit IdentityServer

            dependencyManager.RegisterSingleSignOnServer<CrmPortalUserService, CrmPortalClientsProvider>();

            #endregion

            #region Entity Framework Core

            services
                .AddDbContext<CrmPortalDbContext>(config => config.UseSqlServer(appEnv.GetConfig<string>("AppConnectionString")))
                .AddEntityFrameworkSqlServer();

            #endregion

            dependencyManager.Register<ISmsService, KaveNegarSmsService>();
            dependencyManager.Register<ICustomersRepository, CustomersRepository>();

            dependencyManager.RegisterHangfireBackgroundJobWorkerUsingDefaultConfiguration<JobSchedulerInMemoryBackendConfiguration>();

            dependencyManager.Register(new[] { typeof(IAppEvents).GetTypeInfo(), typeof(SampleJobWhichNeedsInitialization).GetTypeInfo() }, typeof(SampleJobWhichNeedsInitialization).GetTypeInfo());
            dependencyManager.Register<SampleJobWhichDoesNotNeedInitialization>();
            dependencyManager.RegisterAppEvents<HangfireJobsConfiguration>();
        }
    }

    public class SampleJobWhichNeedsInitialization : IAppEvents
    {
        private static /*static is important here!*/ string someThingWhichGetsResolvedDuringInitialization;

        public void OnAppEnd()
        {

        }

        public void OnAppStartup()
        {
            // init async
            someThingWhichGetsResolvedDuringInitialization = "Test";
        }

        public IDateTimeProvider DateTimeProvider { get; set; }

        public async Task Do()
        {
            Console.WriteLine($"SampleJobWhichNeedsInitialization => {DateTimeProvider.GetCurrentUtcDateTime()} | {someThingWhichGetsResolvedDuringInitialization}");
        }
    }

    public class SampleJobWhichDoesNotNeedInitialization
    {
        public IDateTimeProvider DateTimeProvider { get; set; }

        public CrmPortalDbContext DbContext { get; set; }

        public async Task Do()
        {
            Console.WriteLine($"SampleJobWhichDoesNotNeedInitialization => {DateTimeProvider.GetCurrentUtcDateTime()}");
        }
    }

    public class HangfireJobsConfiguration : IAppEvents
    {
        public IBackgroundJobWorker BackgroundJobWorker { get; set; }

        public void OnAppStartup()
        {
            BackgroundJobWorker.PerformRecurringBackgroundJob<SampleJobWhichNeedsInitialization>("SampleJobWhichNeedsInitialization", job => job.Do(), Cron.Minutely());
            BackgroundJobWorker.PerformRecurringBackgroundJob<SampleJobWhichDoesNotNeedInitialization>("SampleJobWhichDoesNotNeedInitialization", job => job.Do(), Cron.Minutely());
        }

        public void OnAppEnd()
        {
        }
    }
}
