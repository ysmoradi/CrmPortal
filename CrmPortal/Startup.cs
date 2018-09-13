using Bit.Core;
using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Bit.Core.Models;
using Bit.Owin.Implementations;
using Bit.OwinCore;
using CrmPortal.Api.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Reflection;

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
                        EnvironmentAppInfo appInfo = DefaultAppEnvironmentsProvider.Current.GetActiveAppEnvironment().AppInfo;
                        c.SingleApiVersion($"v{appInfo.Version}", $"{appInfo.Name}-Api");
                        c.ApplyDefaultApiConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });

                webApiDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();
            });

            #endregion

            #region Bit IdentityServer

            dependencyManager.RegisterSingleSignOnServer<CrmPortalUserService, CrmPortalClientsProvider>();

            #endregion
        }
    }
}
