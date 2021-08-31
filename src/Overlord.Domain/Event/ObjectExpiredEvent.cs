using Overlord.Core.Entities.Frame;

namespace Overlord.Domain.Event
{
    public class ObjectExpiredEvent : EventBase
    {
        private readonly string _id;
        private readonly int _typeId;
        private readonly string _type;
        private readonly long _trackingId;

        public string Id => _id;
        public int TypeId => _typeId;
        public string Type => _type;
        public long TrackingId => _trackingId;

        public ObjectExpiredEvent(string id, int typeId, string type, long trackingId)
        {
            _id = id;
            _typeId = typeId;
            _type = type;
            _trackingId = trackingId;
        }

        public ObjectExpiredEvent(TrafficObjectInfo toi)
        {
            _id = toi.Id;
            _typeId = toi.TypeId;
            _type = toi.Type;
            _trackingId = toi.TrackingId;
        }
    }
}
