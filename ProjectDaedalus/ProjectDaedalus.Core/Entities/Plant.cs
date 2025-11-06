using System;
using System.Collections.Generic;

namespace ProjectDaedalus.Core.Entities;

public class Plant
{
    public int PlantId { get; set; }

    public required string ScientificName { get; set; }

    public required string FamiliarName { get; set; }

    public decimal MoistureLowRange { get; set; }

    public decimal MoistureHighRange { get; set; }

    public string? FunFact { get; set; }

    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
}
