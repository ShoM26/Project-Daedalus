using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectDaeadalus.Scripts.Services;
using ProjectDaedalus.Scripts.Services;
using ProjectDaedalus.API.Dtos.Plant;

namespace ProjectDaeadalus.Scripts.Scripts
{
    public interface IBulkPlantRegistration
    {
        Task<BulkRegistrationResult> ExecuteAsync(string csvFilePath);
    }
    public class BulkPlantRegistration : IBulkPlantRegistration
    {
        private readonly IInternalApiService _apiService;
        private readonly ILogger<BulkPlantRegistration> _logger;

        public BulkPlantRegistration(IInternalApiService apiService, ILogger<BulkPlantRegistration> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }
        
        public async Task<BulkRegistrationResult> ExecuteAsync(string csv)
        {
            _logger.LogInformation($"Starting Bulk plant registration on {csv}");

            try
            {
                var plantDtos = ParseCsvToPlantDtos(csv);

                if (plantDtos.Count == 0)
                {
                    _logger.LogWarning("No valid plants in csv");
                    return new BulkRegistrationResult
                    {
                        TotalPlants = 0,
                        ErrorMessage = "No valid plants in csv"
                    };
                }

                _logger.LogInformation("Parsed {Count} plants from csv", plantDtos.Count);

                var result = await _apiService.BulkRegisterPlantsAsync(plantDtos);

                _logger.LogInformation("Bulk plant registration complete");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk plant registration failed");
                return new BulkRegistrationResult
                {
                    TotalPlants = 0,
                    FailedRegistrations = 0,
                    ErrorMessage = $"Script error:  {ex.Message}"
                };
            }
        }

        private List<PlantDto> ParseCsvToPlantDtos(string csv)
        {
            var plantDtos = new List<PlantDto>();
            var lines = File.ReadAllLines(csv);

            if (lines.Length < 1)
            {
                _logger.LogWarning("empty csv");
                return plantDtos;
            }
            
            _logger.LogInformation("processing {Count} plants from csv", lines.Length);
            //Open the file
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (string.IsNullOrEmpty(line))
                {
                    _logger.LogDebug("Skipping empty line {linenumber}", i);
                    continue;
                }

                try
                {
                    var plantDto = ParseCsvLineToPlantDto(line, i);
                    if (plantDto != null)
                    {
                        plantDtos.Add(plantDto);
                        _logger.LogInformation("Parsed {}", plantDto.ScientificName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Faied to parse line {linenumber} - {error}", i+1, ex.Message);
                }
            }
            _logger.LogInformation("Finished processing {Count} plants from csv", plantDtos.Count);
            return plantDtos;
        }

        private PlantDto ParseCsvLineToPlantDto(string line, int lineNumber)
        {
            var columns = ParseCsvLine(line);

            if (columns.Length < 4)
            {
                _logger.LogWarning("Not enough columns for plant at line {}", lineNumber);
                return null;
            }

            var scientificName = columns[0]?.Trim();
            var commonName = columns[1]?.Trim();

            if (string.IsNullOrWhiteSpace(scientificName) || string.IsNullOrWhiteSpace(commonName))
            {
                _logger.LogWarning("Missing required fields scientific name or common name");
                return null;
            }

            if (!int.TryParse(columns[2]?.Trim(), out int idealMoistureMin) ||
                !int.TryParse(columns[3]?.Trim(), out int idealMoistureMax))
            {
                _logger.LogWarning("Invalid mositure values at line {linenumber}",  lineNumber);
                return null;
            }

            if (idealMoistureMax < idealMoistureMin)
            {
                _logger.LogWarning(
                    "moisture min greater than moisture max, or moisture min too log, or mositure max too high at line {linenumber}",
                    lineNumber);
                return null;
            }

            return new PlantDto
            {
                ScientificName = scientificName,
                FamiliarName = commonName,
                MoistureLowRange = idealMoistureMin,
                MoistureHighRange = idealMoistureMax,
                FunFact = columns.Length > 4 ? columns[4]?.Trim() : null
            };

        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var currentField = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }
            result.Add(currentField);
            
            return result.ToArray();
        }
        
    }
}
