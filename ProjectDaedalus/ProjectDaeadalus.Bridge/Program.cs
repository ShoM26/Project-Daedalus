using ProjectDaeadalus.Bridge.Services;
using ProjectDaeadalus.Bridge.Models;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectDaeadalus.Bridge.Configuration;
using ProjectDaedalus.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;


namespace ProjectDaeadalus.Bridge
{
    
    /// <summary>
    /// Entry point for the Arduino Bridge Console Application
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true);
            var sharedConfig = new BridgeConfig();
            builder.Configuration.Bind(sharedConfig);
            builder.Services.AddSingleton(sharedConfig);
            builder.Services.AddHttpClient<IInternalApiService, InternalApiService>(client =>
            {
                client.BaseAddress = new Uri(sharedConfig.ApiBaseUrl ?? "http://localhost:5278");
            });
            builder.Services.AddSingleton<BridgeService>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
    
            var app = builder.Build();
            app.UseCors("AllowAll");

            app.MapPost("/setup", async (TokenPayload payload, BridgeConfig config) =>
            {
                Console.WriteLine($"[Setup] Recieved User Token!");
                config.UserToken = payload.Token;   
                return Results.Ok(new { message = "Bridge Configured Successfully!" });
            });
            _ = app.RunAsync("http://localhost:5000");
    
            Console.WriteLine(">> Listening for setup commands on port 5000");

            try
            {
                var bridgeService = app.Services.GetRequiredService<BridgeService>();
                AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
                {
                    bridgeService.Dispose();
                    Console.WriteLine($"[Exit] Process shutting down!");
                };
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