namespace PinBot.Models;

#pragma warning disable CS8618

public class AppConfig
{
    public string Token { get; set; }
    public string ConnectionString { get; set; }
    public LogLevel? LogLevel { get; set; }
    public ulong? ServerId { get; set; }
}
