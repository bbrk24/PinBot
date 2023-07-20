using DataOnion.db;
using Microsoft.EntityFrameworkCore;

namespace PinBot.Models;

public enum Setting
{
    PinEmoji,
    UnpinEmoji,
    ForumsOnly,
}

public class ServerSettings : IEntity<long>
{
    public long Id { get; set; }

    [Unicode]
    public string PinEmoji { get; set; } = "📌";

    [Unicode]
    public string UnpinEmoji { get; set; } = "🚫";

    public bool ForumsOnly { get; set; } = false;

    public override string ToString() => $@"Pin emoji: {PinEmoji}
Unpin Emoji: {UnpinEmoji}
Only check forum threads: {(ForumsOnly ? "yes" : "no (check all threads)")}";
}

