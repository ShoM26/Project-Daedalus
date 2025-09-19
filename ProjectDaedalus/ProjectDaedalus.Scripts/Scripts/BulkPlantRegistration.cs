using System;
using ProjectDaedalus.API.Dtos.Plant;

namespace Scripts.Scripts
{
    public class BulkPlantRegistration
    {
        public async Task RunAsync(string csv)
        {
            //Something that takes a csv file and parses it
            var collection = ParseCsv(csv);
            
            await BulkInsert(collection);
        }

        private IEnumerable<PlantDto> ParseCsv(string csv)
        {
            IEnumerable<PlantDto> plantDtos = IEnumerable<PlantDto>;
            //Open the file
            foreach (var line in csv.Split('\n'))
            {
                //parsing
                var dto = new PlantDto
                {
                    
                };
                plantDtos.Add(dto);
            }
            return plantDtos;
        }

        private async Task BulkInsert(IEnumerable<PlantDto> plantDtos)
        {
            //Repo call
            await _internalApiService.PostManyAsync<object>("path",plantDtos);
        }
    }
}
