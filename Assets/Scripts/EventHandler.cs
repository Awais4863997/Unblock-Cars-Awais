using System;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour, IEventHandler
{
    private readonly Dictionary<string, Action> _events = new Dictionary<string, Action>();

    

    public void Publish(string eventId)
    {
        if (_events.TryGetValue(eventId, out var handlers))
            handlers.Invoke();
    }

    public void Subscribe(string eventId, Action handler)
    {
        if (!_events.ContainsKey(eventId))
            _events[eventId] = handler;
        else
            _events[eventId] += handler;
    }

    public void Unsubscribe(string eventId, Action handler)
    {
        if (_events.ContainsKey(eventId))
            _events[eventId] -= handler;
    }


}