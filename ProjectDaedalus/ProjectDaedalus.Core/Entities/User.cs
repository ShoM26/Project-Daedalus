using System;
using System.Collections.Generic;

namespace ProjectDaedalus.Core.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
