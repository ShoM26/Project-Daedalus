namespace ProjectDaedalus.API.Dtos
{
    public class PlantDTO
    {
        public int? PlantId { get; set; } //Nullable for post and put requests

        public string ScientificName { get; set; } = string.Empty;

        public string FamiliarName { get; set; } = string.Empty;

        public decimal MoistureLowRange { get; set; }

        public decimal MoistureHighRange { get; set; }

        public string? FunFact { get; set; }
    
    }
}

