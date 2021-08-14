using Bit.Core;
using Bit.Core.Contracts;
using Bit.Owin;
using Bit.Owin.Implementations;
using CrmPortal;
using CrmPortal.Api.Contracts;
using CrmPortal.Api.Implementations;
using CrmPortal.Data;
using CrmPortal.Data.Contracts;
using CrmPortal.Data.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;

[assembly: AppModule(typeof(CrmPortalDependencies))]

namespace CrmPortal
{
    public class Startup : AspNetCoreAppStartup
    {

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
                    httpConfiguration.EnableMultiVersionWebApiSwaggerWithUI();
                });

                webApiDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();
            });

            #endregion

            #region Bit IdentityServer

            dependencyManager.RegisterSingleSignOnServer<CrmPortalUserService, CrmPortalClientsProvider>();

            #endregion

            #region Entity Framework Core

            dependencyManager
                .RegisterEfCoreDbContext<CrmPortalDbContext>((serviceProvider, config) => config.UseSqlServer(serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("AppConnectionString")));

            #endregion

            dependencyManager.Register<ISmsService, KaveNegarSmsService>();
            dependencyManager.Register<ICustomersRepository, CustomersRepository>();
        }
    }
}
