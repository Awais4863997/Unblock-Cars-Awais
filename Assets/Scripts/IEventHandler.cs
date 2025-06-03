using System;

public interface IEventHandler
{
    void Publish(string eventId);
    void Subscribe(string eventId, Action handler);
    void Unsubscribe(string eventId, Action handler);


}