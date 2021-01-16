using Microsoft.Extensions.Configuration;
using System.IO;

namespace CrmPortal.Util
{
    public static class CrmPortalConfigurationProvider
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();
        }
    }
}
