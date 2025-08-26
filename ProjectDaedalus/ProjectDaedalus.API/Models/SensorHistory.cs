using System;
using System.Collections.Generic;

namespace ProjectDaedalus.API.Models;

public partial class SensorHistory
{
    public DateTime TimeStamp { get; set; }

    public int DeviceId { get; set; }

    public decimal MoistureLevel { get; set; }

    public virtual Device Device { get; set; } = null!;
}
