using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Infrastructure.Data;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Repositories;
using ProjectDaedalus.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core DbContext (pointing at MySQL)
builder.Services.AddDbContext<DaedalusContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

// Register repositories (so UnitOfWork can inject them)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IUserPlantRepository, UserPlantRepository>();

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Adding CORS to allow front end to call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactApp");

app.Use(async (context, next) =>
{
    Console.WriteLine($"=== Incoming Request ===");
    Console.WriteLine($"Method: {context.Request.Method}");
    Console.WriteLine($"Path: {context.Request.Path}");
    Console.WriteLine($"Full URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
    await next();
});

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();