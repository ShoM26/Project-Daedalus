using System;
using System.Collections.Generic;

namespace ProjectDaedalus.API.Models;

public partial class Plant
{
    public int PlantId { get; set; }

    public string ScientificName { get; set; } = null!;

    public string FamiliarName { get; set; } = null!;

    public decimal MoistureLowRange { get; set; }

    public decimal MoistureHighRange { get; set; }

    public string? FunFact { get; set; }

    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
}
