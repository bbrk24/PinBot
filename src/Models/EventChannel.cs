using DataOnion.db;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinBot.Models;

public enum EventName
{
    Start,
}

public class EventChannel : IEntity<EventName>
{
    [Column(TypeName = "text")]
    public EventName Id { get; set; }
    public long Channel { get; set; }
}
