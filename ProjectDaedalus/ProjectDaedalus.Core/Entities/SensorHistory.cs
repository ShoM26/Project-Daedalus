namespace ProjectDaedalus.Core.Entities;

public class SensorHistory
{
    public int DeviceId { get; set; } //fk
    public DateTime TimeStamp { get; set; }
    public decimal MoistureLevel { get; set; }
    public virtual Device Device { get; set; }
}
