namespace HackerNews.API.Application.Dispatchers;


public interface IEvent
{
    int EventId { get; }
}

/// <summary>
/// Used to encapsulate a single event by its unique Id and then used by Event Delegates
/// </summary>
public class Event : IEvent
{
    public Event(int eventId)
    {
        EventId = eventId;
    }

    public int EventId { get; }

    protected bool Equals(Event other)
    {
        return EventId == other.EventId;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Event)obj);
    }

    public override int GetHashCode()
    {
        return EventId;
    }
}