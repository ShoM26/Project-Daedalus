namespace ProjectDaedalus.API.Dtos.Plant
{
    public class PlantDto
    {
        public int? PlantId { get; set; }

        public string ScientificName { get; set; } = string.Empty;

        public string FamiliarName { get; set; } = string.Empty;

        public decimal MoistureLowRange { get; set; }

        public decimal MoistureHighRange { get; set; }

        public string? FunFact { get; set; }
    
    }
}

