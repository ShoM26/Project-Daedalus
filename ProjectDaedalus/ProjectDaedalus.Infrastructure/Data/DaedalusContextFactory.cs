using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectDaedalus.Infrastructure.Data;

public class DaedalusContextFactory : IDesignTimeDbContextFactory<DaedalusContext>
{
    public DaedalusContext CreateDbContext(string[] args)
    {
        // Build configuration to read from appsettings.Development.json
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..");
        basePath = Path.Combine(basePath, "ProjectDaedalus.API");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Build DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<DaedalusContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new DaedalusContext(optionsBuilder.Options);
    }
}