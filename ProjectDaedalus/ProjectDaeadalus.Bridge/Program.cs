using ProjectDaeadalus.Bridge.Services;
using ProjectDaeadalus.Bridge.Models;
using System;
using System.Threading.Tasks;

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
            Console.WriteLine("Starting Arduino to API bridge service...\n");

            try
            {
                // Create and run the bridge service
                var bridgeService = new BridgeService();
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