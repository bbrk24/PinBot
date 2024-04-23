using DataOnion;
using Microsoft.EntityFrameworkCore;
using PinBot.Models;
using PinBot.Services;
using Npgsql;
using PinBot.Repositories;
using Microsoft.Extensions.Logging.Console;
using System.Diagnostics.CodeAnalysis;

namespace PinBot;

public static class Setup
{
    private static readonly AppConfig _config = new();

    private static void SetFormatterOptions(ConsoleFormatterOptions opt)
    {
        opt.TimestampFormat = "[yyyy-MM-ddTHH:mm:ss.ff] ";
        opt.UseUtcTimestamp = true;
    }

    public static void AddLogger(this IServiceCollection builder)
    {
        builder.AddLogging(logBuilder =>
        {
            logBuilder.AddJsonConsole(SetFormatterOptions)
                .AddSystemdConsole(SetFormatterOptions)
                .AddSimpleConsole(SetFormatterOptions);

            if (_config.LogLevel.HasValue)
            {
                logBuilder.SetMinimumLevel(_config.LogLevel.Value);
            }
        });

        builder.AddScoped(typeof(ILogger<>), typeof(Logger<>));
        builder.AddScoped<ILoggingService, LoggingService>();
    }

    private static T Unwrap<T>([NotNull] T? value)
        where T : class
    {
        return value ?? throw new NullReferenceException();
    }

    public static void AddConfig(this IServiceCollection builder)
    {
        _config.Token = Unwrap(Environment.GetEnvironmentVariable("TOKEN"));
        _config.ConnectionString = Unwrap(
            Environment.GetEnvironmentVariable("CONNECTION_STRING")
        );

        if (Enum.TryParse(
            Environment.GetEnvironmentVariable("LOG_LEVEL"),
            out LogLevel logLevel
        ))
        {
            _config.LogLevel = logLevel;
        }

        if (ulong.TryParse(Environment.GetEnvironmentVariable("GUILD"), out var serverId))
        {
            _config.ServerId = serverId;
        }

        builder.AddSingleton(_config);
    }

    public static void AddDatabase(this IServiceCollection builder)
    {
        builder.AddDatabaseOnion(_config.ConnectionString)
            .ConfigureDapper(str => new NpgsqlConnection(str))
            .ConfigureEfCore<ApplicationContext>(
                connStr => optBuilder => optBuilder.UseNpgsql(connStr)
            );
    }

    public static void AddRepositories(this IServiceCollection builder)
    {
        builder.AddScoped<IEventChannelRepository, EventChannelRepository>();
        builder.AddScoped<IServerSettingsRepository, ServerSettingsRepository>();
    }

    public static void AddGeneralServices(this IServiceCollection builder)
    {
        builder.AddScoped<IMessageService, MessageService>();
        builder.AddScoped<ICommandsService, CommandsService>();
        builder.AddScoped<IEventsService, EventsService>();
        builder.AddScoped<ISettingsService, SettingsService>();
        builder.AddSingleton<IReadyHandler, ReadyHandler>();
        builder.AddSingleton<IRoombaService, RoombaService>();
    }

    public static void AddRoombas(this IServiceCollection builder)
    {
        builder.AddScoped<IRoomba, ServerSettingsRepository>();
        builder.AddScoped<IRoomba, EventChannelRoomba>();
    }
}
