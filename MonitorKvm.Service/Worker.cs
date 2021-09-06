using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitorKvm.Core;
using MonitorKvm.Core.Listener;
using MonitorKvm.UsbProvider;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorKvm.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _Logger;

        public Worker(ILogger<Worker> logger, String[] args)
        {
            _Logger = logger;
            this._Args = args;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var title = $"Monitor KVM {version?.Major}.{version?.Minor}.{version?.Build}";
            
            Console.Title = title;
            logger.LogInformation(title);
        }

        
        private String[] _Args;
        private Listener _Listener;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._Listener = new Listener(this._Logger, this._Args[0]);
            this._Logger.LogInformation($"Running usb listener for usb device: '{this._Args[0]}'");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                this._Logger.LogInformation("Polling for change...");
                this._Listener.PollForChange();
            }
        }
    }
}
