using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace MonitorKvm.Service
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "1");
            Program.CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(String[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>(serviceProvider =>
                    {
                        return new Worker(serviceProvider.GetService<ILogger<Worker>>(), args);
                    });
                });
    }
}
