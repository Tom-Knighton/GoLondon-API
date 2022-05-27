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
            IAPIClient apiClient = scope.ServiceProvider.GetRequiredService<IAPIClient>();

            string[] lineIdsToGrab = new string[] { "bakerloo", "central", "circle", "district", "dlr", "elizabeth", "hammersmith-city", "jubilee", "london-overground", "metropolitan", "northern", "tram", "piccadilly", "victoria", "waterloo-city" };

            List<Task<LineRoutes>> tasksToDo = new();
            foreach (string id in lineIdsToGrab)
            {

                tasksToDo.Add(apiClient.PerformAsync<LineRoutes>(Domain.Enums.APIClientType.TFL, $"Line/{id}/Route/sequence/outbound"));
            }

            List<LineRoutes> routes = (await Task.WhenAll(tasksToDo)).ToList();
            Global.UpdateCachedLineRoutes(routes);

            await metaService.SyncWithTfl();

            Console.WriteLine("Route and TfL sync complete");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

