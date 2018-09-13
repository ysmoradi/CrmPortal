using Bit.Test;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CrmPortal.Test
{
    public class CrmPortalTestEnv : TestEnvironmentBase
    {
        public CrmPortalTestEnv(TestEnvironmentArgs args = null)
            : base(ApplyArgsDefaults(args))
        {

        }

        private static TestEnvironmentArgs ApplyArgsDefaults(TestEnvironmentArgs args)
        {
            args = args ?? new TestEnvironmentArgs();
            args.CustomAppModulesProvider = args.CustomAppModulesProvider ?? new CrmPortalDependencies();
            args.UseAspNetCore = true;
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
