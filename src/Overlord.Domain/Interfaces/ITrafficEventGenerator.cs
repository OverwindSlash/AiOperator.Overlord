using Overlord.Domain.Event;

namespace Overlord.Domain.Interfaces
{
    public interface ITrafficEventGenerator
    {
        TrafficEvent CreateForbiddenEvent(string deviceNo, int laneIndex, int typeId, long trackingId);
        TrafficEvent CreateStoppedEvent(string deviceNo, int laneIndex, int typeId, long trackingId);
        TrafficEvent CreateSlowEvent(string deviceNo, int laneIndex, int typeId, long trackingId);
        TrafficEvent CreateAmbleEvent(string deviceNo, int laneIndex, int typeId, long trackingId);
        TrafficEvent CreateJamEvent(string deviceNo, int laneIndex, int typeId, long trackingId);
    }
}
