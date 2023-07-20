using Microsoft.EntityFrameworkCore;
using PinBot.Models;

namespace PinBot;

public class ApplicationContext : DbContext
{
    private readonly AppConfig _config;

#pragma warning disable CS8618
    public ApplicationContext(
        AppConfig config
    )
    {
        _config = config;
    }

    public DbSet<EventChannel> EventChannels { get; set; }
    public DbSet<ServerSettings> ServerSettings { get; set; }
#pragma warning restore

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(_config.ConnectionString)
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention();
    }
}
