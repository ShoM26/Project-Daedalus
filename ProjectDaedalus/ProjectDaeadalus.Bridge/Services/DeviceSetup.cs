using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ProjectDaeadalus.Bridge.Configuration;

namespace ProjectDaeadalus.Bridge.Services
{
    public class DeviceSetup
    {
        private const string ConfigFileName = "config.json";
        private const string SetupUrl = "";

        /// <summary>
        /// Checks for config file. If missing, starts the 'Magic Button' server and waits for the frontend.
        /// </summary>
        public static async Task<BridgeConfig> EnsureConfiguredAsync()
        {
            //Check if config file already exists
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    string exisitingJson = File.ReadAllText(ConfigFileName);
                    var config = JsonSerializer.Deserialize<BridgeConfig>(exisitingJson);
                    Console.WriteLine($"[Config] Loaded configuration for API: {config.ApiBaseUrl}");
                    return config;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Config] Error reading config {e.Message}");
                }
            }

            //Run startup server to generate config file
            return await RunSetupServerAsync();
        }

        private static async Task<BridgeConfig> RunSetupServerAsync()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("   DAEDALUS BRIDGE - FIRST TIME SETUP");
            Console.WriteLine("=================================================");
            Console.WriteLine($"[1] Open your Web Dashboard.");
            Console.WriteLine($"[2] Click 'Connect Bridge' in the settings.");
            Console.WriteLine($"[Waiting] Listening for token on {SetupUrl}...");
            
            using var listener = new HttpListener();
            listener.Prefixes.Add(SetupUrl);
            listener.Start();
            
            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;
            
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = 204;
                response.Close();
                return await RunSetupServerAsync();
            }
            using var reader = new StreamReader(request.InputStream);
            string jsonBody = await reader.ReadToEndAsync();
            
            var incomingData = JsonSerializer.Deserialize<BridgeConfig>(jsonBody);

            if (string.IsNullOrEmpty(incomingData?.UserToken))
            {
                Console.WriteLine("[Error] Recieved empty token. Retrying");
                response.StatusCode = 400;
                response.Close();
                return await RunSetupServerAsync();
            }

            var finalConfig = new BridgeConfig
            {
                UserToken = incomingData.UserToken,
                ApiBaseUrl = !string.IsNullOrEmpty(incomingData.ApiBaseUrl)
                    ? incomingData.ApiBaseUrl
                    : "http://localhost:5278"
            };
            
            string saveJson = JsonSerializer.Serialize(finalConfig, new  JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFileName, saveJson);
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Bridge Configured Successfully");
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
            
            listener.Stop();
            Console.WriteLine("[Config] Successfully configured.");
            return finalConfig;
        }
}
}

