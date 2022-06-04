using System;
using System.Xml;
using System.Xml.Serialization;
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

            // Get Route data
            string[] lineIdsToGrab = new string[] { "bakerloo", "central", "circle", "district", "dlr", "elizabeth", "hammersmith-city", "jubilee", "london-overground", "metropolitan", "northern", "tram", "piccadilly", "victoria", "waterloo-city" };

            List<Task<LineRoutes>> tasksToDo = new();
            foreach (string id in lineIdsToGrab)
            {

                tasksToDo.Add(apiClient.PerformAsync<LineRoutes>(Domain.Enums.APIClientType.TFL, $"Line/{id}/Route/sequence/outbound"));
            }

            List<LineRoutes> routes = (await Task.WhenAll(tasksToDo)).ToList();
            Global.UpdateCachedLineRoutes(routes);

            // Sync with TfL
            await metaService.SyncWithTfl();

            // Read Irad data
            XmlReader reader = XmlReader.Create("https://content.tfl.gov.uk/lrad-v2.xml");
            XmlSerializer serialiser = new(typeof(AccessibilityLinkRoot));
            AccessibilityLinkRoot root = (AccessibilityLinkRoot)serialiser.Deserialize(reader);

            List<StopPointAccessibility> iradData = new();
            foreach (AccessibilityLink link in root?.Stations)
            {
                iradData.Add(new StopPointAccessibility(link));
            }

            Global.UpdateIradCache(iradData);

            // Finish
            Console.WriteLine("Route, Accessibility/Irad and TfL sync complete");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

