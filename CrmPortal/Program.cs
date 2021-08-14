using Bit.Core;
using Bit.Owin;
using Bit.Owin.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace CrmPortal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AssemblyContainer.Current.Init();

            AspNetCoreAppEnvironmentsProvider.Current.Use();

            await CreateWebHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            BitWebHost.CreateWebHost<Startup>(args);
    }
}
