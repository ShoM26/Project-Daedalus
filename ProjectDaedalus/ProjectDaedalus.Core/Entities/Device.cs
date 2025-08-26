using System;
using System.Collections.Generic;

namespace ProjectDaedalus.Core.Entities;

public partial class Device
{
    public int DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public string ConnectionType { get; set; } = null!;

    public string ConnectionAddress { get; set; } = null!;

    public DateTime? LastSeen { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<SensorHistory> SensorHistories { get; set; } = new List<SensorHistory>();

    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
}
