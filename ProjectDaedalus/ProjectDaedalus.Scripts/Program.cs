using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectDaedalus.Scripts.Servicies;
using Microsoft.Extensions.DependencyInjection;
using ProjectDaeadalus.Scripts.Services;

namespace ProjectDaeadalus.Scripts
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Internal Scripts");
            Console.WriteLine("=================\n");

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                        optional: true)
                    .Build();

                var services = new ServiceCollection();
                ConfigureServices(services, configuration);

                var serviceProvider = services.BuildServiceProvider();

                //Test API connection
                var apiService = serviceProvider.GetRequiredService<IInternalApiService>();
                var connectionTest = await apiService.TestConnectionAsync();

                if (!connectionTest)
                {
                    Console.WriteLine("Could not connect to API");
                    return;
                }

                await HandleCommandLineArgs(serviceProvider, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application Error: {ex.Message}");
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(configuration.GetSection("Logging"));
            });
            
            services.AddSingleton<IInternalApiService, InternalApiService>();

            services.AddHttpClient<IInternalApiService, InternalApiService>();
            
            services.AddScoped<IInternalApiService, InternalApiService>();
            
            //Add scripts here
        }
        

        private static async Task HandleCommandLineArgs(IServiceProvider serviceProvider, string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet run <command> <args>");
                return;
            }
            
            var command =  args[0].ToLower();

            switch (command)
            {
                case "bulk-plants":
                    if (args.Length > 1)
                    {
                        await RunBulkPlantRegistration(serviceProvider, args[1]);
                    }
                    else
                    {
                        Console.WriteLine("csv file path required for bulk insert");
                        Console.WriteLine("Usage: dotnet run bulk-plants 'path/to/csv'");
                    }
                    break;
                case "-help":case "-h":
                    Console.WriteLine("Usage: dotnet run <command> <args>");
                    break;
                default:
                    Console.WriteLine("Invalid command");
                    Console.WriteLine("Usage: dotnet run <command> <args>");
                    break;
            }
        }

        private static async Task RunBulkPlantRegistration(IServiceProvider serviceProvider, string csvFilePath = null)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath))
                {
                    Console.WriteLine("CSV file path does not exist");
                    return;
                }

                Console.WriteLine($"Processing CSV file: {csvFilePath}");
                //Actual method implementation
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in bulk plant registration");
            }
        }
    }
}