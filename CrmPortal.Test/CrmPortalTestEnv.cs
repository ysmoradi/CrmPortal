﻿using Bit.Owin.Implementations;
using Bit.Test;
using CrmPortal.Util;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CrmPortal.Test
{
    public class CrmPortalTestEnv : TestEnvironmentBase
    {
        static CrmPortalTestEnv()
        {
            if (!Environment.Is64BitProcess)
                throw new InvalidOperationException("Please run tests in x64 process");

            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../CrmPortal");
            AspNetCoreAppEnvironmentsProvider.Current.Configuration = CrmPortalConfigurationProvider.GetConfiguration();
            IHostEnvironment hostEnv = A.Fake<IHostEnvironment>();
            hostEnv.EnvironmentName = Environments.Development;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);
            hostEnv.ApplicationName = "CrmPortal";
            AspNetCoreAppEnvironmentsProvider.Current.HostingEnvironment = hostEnv;
            AspNetCoreAppEnvironmentsProvider.Current.Init();
            AspNetCoreAppEnvironmentsProvider.Current.Use();
        }

        public CrmPortalTestEnv(TestEnvironmentArgs args = null)
            : base(ApplyArgsDefaults(args))
        {

        }

        private static TestEnvironmentArgs ApplyArgsDefaults(TestEnvironmentArgs args)
        {
            args = args ?? new TestEnvironmentArgs();
            args.CustomAppModulesProvider = args.CustomAppModulesProvider ?? new CrmPortalDependencies();
            return args;
        }

        protected override List<Func<TypeInfo, bool>> GetAutoProxyCreationIncludeRules()
        {
            List<Func<TypeInfo, bool>> baseList = base.GetAutoProxyCreationIncludeRules();

            baseList.Add(implementationType => implementationType.Assembly == typeof(Startup).GetTypeInfo().Assembly);

            return baseList;
        }
    }
}
