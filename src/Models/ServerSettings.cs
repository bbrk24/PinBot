using DataOnion.db;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinBot.Models;

public class ServerSettings : IEntity<long>
{
    public long Id { get; set; }

    [Unicode]
    public string PinEmoji { get; set; } = "📌";

    [Unicode]
    public string UnpinEmoji { get; set; } = "🚫";

    [Column(TypeName = "boolean")]
    public bool ForumsOnly { get; set; } = false;

    [Column(TypeName = "boolean")]
    public bool IgnoreBots { get; set; } = false;

    public override string ToString() => $@"Pin emoji: {PinEmoji}
Unpin Emoji: {UnpinEmoji}
Only check forum threads: {(ForumsOnly ? "yes" : "no (check all threads)")}
Ignore reactions from bots: {(IgnoreBots ? "yes" : "no")}";
}

