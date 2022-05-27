using System;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;

namespace GoLondonAPI.Services
{
    public class GLBackgroundService : BackgroundService
    {

        private readonly IServiceProvider _services;

        public GLBackgroundService(IServiceProvider servicesProvider)
        {
            _services = servicesProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoTasks();
            PeriodicTimer timer = new(TimeSpan.FromHours(24));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoTasks();
            }
        }

        private async Task DoTasks()
        {
            using var scope = _services.CreateAsyncScope();
            IMetaService metaService = scope.ServiceProvider.GetRequiredService<IMetaService>();           

            await metaService.SyncWithTfl();

            Console.WriteLine("TfL sync complete");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

