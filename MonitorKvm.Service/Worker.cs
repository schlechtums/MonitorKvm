using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitorKvm.Service.Types;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorKvm.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var title = $"Monitor KVM {version?.Major}.{version?.Minor}.{version?.Build}";
            
            Console.Title = title;
            logger.LogInformation(title);

            new NAudioEndpointNotificationHandler(logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
