using ProjectDaeadalus.Bridge.Services;
using ProjectDaeadalus.Bridge.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectDaeadalus.Bridge.Configuration;
using ProjectDaedalus.Core.Entities;


namespace ProjectDaeadalus.Bridge
{
    /// <summary>
    /// Entry point for the Arduino Bridge Console Application
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Project Daedalus Arduino Bridge ===");

            var handshakeConfig = await DeviceSetup.EnsureConfiguredAsync();
            
            Console.WriteLine("Starting Arduino to API bridge service...\n");

            try
            {
                // Build configuration
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                // Build service provider
                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton(handshakeConfig);
                services.AddHttpClient<IInternalApiService, InternalApiService>(client =>
                {
                    client.BaseAddress = new Uri(handshakeConfig.ApiBaseUrl);
                });
                services.AddSingleton<BridgeService>();
                var serviceProvider = services.BuildServiceProvider();
                
                // Create and run the bridge service
                var bridgeService = serviceProvider.GetRequiredService<BridgeService>();
                await bridgeService.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

        }
    }
}