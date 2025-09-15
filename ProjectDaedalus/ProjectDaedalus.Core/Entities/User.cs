using System;
using System.Collections.Generic;

namespace ProjectDaedalus.Core.Entities;

public partial class User
{
    public int UserId { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
