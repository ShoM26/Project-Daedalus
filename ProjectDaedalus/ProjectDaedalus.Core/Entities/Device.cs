using System;
using System.Collections.Generic;

namespace ProjectDaedalus.Core.Entities;

public partial class Device
{
    public int DeviceId { get; set; }

    public string HardwareIdentifier { get; set; } = null!;

    public string ConnectionType { get; set; } = null!;

    public string ConnectionAddress { get; set; } = null!;
    public int UserId { get; set; }

    public DateTime? LastSeen { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<SensorHistory> SensorHistories { get; set; } = new List<SensorHistory>();
    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
    public virtual User User { get; set; } = null!;
}
