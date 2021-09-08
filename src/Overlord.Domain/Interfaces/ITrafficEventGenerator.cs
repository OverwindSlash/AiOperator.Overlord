﻿using Overlord.Domain.Event;

namespace Overlord.Domain.Interfaces
{
    public interface ITrafficEventGenerator
    {
        TrafficEvent CreateForbiddenEvent(string deviceNo, int typeId, long trackingId);
        TrafficEvent CreateStoppedEvent(string deviceNo, int typeId, long trackingId);
    }
}