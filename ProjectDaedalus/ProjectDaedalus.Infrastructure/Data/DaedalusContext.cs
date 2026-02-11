using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Infrastructure.Data;

public class DaedalusContext : DbContext
{
    public DaedalusContext()
    {
    }

    public DaedalusContext(DbContextOptions<DaedalusContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Plant> Plants { get; set; }

    public DbSet<SensorHistory> SensorHistories { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserPlant> UserPlants { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationHistory> NotificationHistory { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PRIMARY");

            entity.ToTable("devices");

            entity.HasIndex(e => e.ConnectionAddress, "connection_address").IsUnique();
            entity.HasIndex(e => e.UserId, "fk_device");

            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.ConnectionAddress)
                .HasMaxLength(250)
                .HasColumnName("connection_address");
            entity.Property(e => e.ConnectionType)
                .HasColumnType("enum('USB','Bluetooth','WiFi')")
                .HasColumnName("connection_type");
            entity.Property(e => e.HardwareIdentifier)
                .HasMaxLength(150)
                .HasColumnName("hardware_identifier");
            entity.Property(e=> e.UserId).HasColumnName("user_id");
            entity.Property(e => e.LastSeen)
                .HasColumnType("timestamp")
                .HasColumnName("last_seen");
            entity.Property(e => e.Status)
                .HasColumnType("enum('Active','Inactive','Disconnected')")
                .HasColumnName("status");
            entity.HasOne(u => u.User).WithMany(p => p.Devices).HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade).HasConstraintName("fk_device");
        });

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.PlantId).HasName("PRIMARY");

            entity.ToTable("plants");

            entity.HasIndex(e => e.ScientificName, "scientific_name").IsUnique();

            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.FamiliarName)
                .HasMaxLength(150)
                .HasColumnName("familiar_name");
            entity.Property(e => e.FunFact)
                .HasColumnType("text")
                .HasColumnName("fun_fact");
            entity.Property(e => e.MoistureHighRange)
                .HasPrecision(5, 2)
                .HasColumnName("moisture_high_range");
            entity.Property(e => e.MoistureLowRange)
                .HasPrecision(5, 2)
                .HasColumnName("moisture_low_range");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(150)
                .HasColumnName("scientific_name");
        });

        modelBuilder.Entity<SensorHistory>(entity =>
        {
            entity.HasKey(e => new { e.TimeStamp, e.DeviceId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("sensor_history");

            entity.HasIndex(e => e.DeviceId, "fk_sensor_history");

            entity.Property(e => e.TimeStamp)
                .HasColumnType("timestamp")
                .HasColumnName("time_stamp");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.MoistureLevel)
                .HasPrecision(5, 2)
                .HasColumnName("moisture_level");

            entity.HasOne(d => d.Device).WithMany(p => p.SensorHistories)
                .HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_sensor_history");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();
            
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserPlant>(entity =>
        {
            entity.HasKey(e => e.UserPlantId).HasName("PRIMARY");

            entity.ToTable("user_plants");

            entity.HasIndex(e => e.DeviceId, "fk_userplants_device");

            entity.HasIndex(e => e.PlantId, "fk_userplants_plant");

            entity.HasIndex(e => new { e.UserId, e.DeviceId }, "user_id").IsUnique();

            entity.Property(e => e.UserPlantId).HasColumnName("user_plant_id");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("date_added");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.PlantId).HasColumnName("plant_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Device).WithMany(p => p.UserPlants)
                .HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_userplants_device");

            entity.HasOne(d => d.Plant).WithMany(p => p.UserPlants)
                .HasForeignKey(d => d.PlantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userplants_plant");

            entity.HasOne(d => d.User).WithMany(p => p.UserPlants)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_userplants_user");
        });
        
        modelBuilder.Entity<Notification>(entity =>
    {
        entity.HasKey(n => n.NotificationId);
        
        entity.Property(n => n.NotificationId)
            .HasColumnName("notification_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(n => n.UserPlantId)
            .HasColumnName("userplant_id")
            .IsRequired();
        
        entity.Property(n => n.Message)
            .HasColumnName("message")
            .HasMaxLength(255)
            .IsRequired();
        
        entity.Property(n => n.NotificationType)
            .HasColumnName("notification_type")
            .HasConversion<int>()
            .IsRequired();
        
        entity.Property(n => n.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();
        
        entity.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        entity.HasOne(n => n.UserPlant)
            .WithMany(up => up.Notifications)
            .HasForeignKey(n => n.UserPlantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasIndex(n => new { n.UserPlantId, n.IsRead, n.CreatedAt })
            .HasDatabaseName("idx_notifications_userplant_read_created");
        
        entity.HasIndex(n => n.CreatedAt)
            .HasDatabaseName("idx_notifications_created");
    });
    
    modelBuilder.Entity<NotificationHistory>(entity =>
    {
        entity.HasKey(nh => nh.NotificationHistoryId);
        
        entity.Property(nh => nh.NotificationHistoryId)
            .HasColumnName("notification_history_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(nh => nh.UserPlantId)
            .HasColumnName("user_plant_id")
            .IsRequired();
        
        entity.Property(nh => nh.NotificationType)
            .HasColumnName("notification_type")
            .HasConversion<int>() 
            .IsRequired();
        
        entity.Property(nh => nh.MoistureValue)
            .HasColumnName("moisture_value")
            .HasPrecision(5, 2) 
            .IsRequired();
        
        entity.Property(nh => nh.ThresholdValue)
            .HasColumnName("threshold_value")
            .HasPrecision(5, 2)
            .IsRequired();
        
        entity.Property(nh => nh.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        entity.HasOne(nh => nh.UserPlant)
            .WithMany(up => up.NotificationHistories)
            .HasForeignKey(nh => nh.UserPlantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasIndex(nh => new { nh.UserPlantId, nh.NotificationType, nh.CreatedAt })
            .HasDatabaseName("idx_notification_history_userplant_type_created");
        
        entity.HasIndex(nh => new { nh.UserPlantId, nh.NotificationType })
            .HasDatabaseName("idx_notification_history_dedup");
    });
    } 
}
