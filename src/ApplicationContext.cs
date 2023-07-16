using Microsoft.EntityFrameworkCore;
using PinBot.Models;

namespace PinBot;

#pragma warning disable CS8618

public class ApplicationContext : DbContext
{
    private readonly AppConfig _config;

    public ApplicationContext(
        AppConfig config
    )
    {
        _config = config;
    }

    public DbSet<EventChannel> EventChannels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(_config.ConnectionString)
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention();
    }
}
