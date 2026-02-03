namespace ProjectDaedalus.Core.Entities;

public class Device
{
    public int DeviceId { get; set; }

    public required string HardwareIdentifier { get; set; }

    //USB, Bluetooth, or Wifi
    public required string ConnectionType { get; set; }

    //COM port for USB or MAC address for Bluetooth
    public required string ConnectionAddress { get; set; }
    public required int UserId { get; set; }

    public DateTime? LastSeen { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<SensorHistory> SensorHistories { get; set; } = new List<SensorHistory>();
    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
    public virtual User User { get; set; }
}
