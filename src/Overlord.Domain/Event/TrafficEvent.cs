using System.Text.Json.Serialization;

namespace Overlord.Domain.Event
{
    public class TrafficEvent : EventBase
    {
        [JsonIgnore]
        public string DeviceNo { get; set; }

        [JsonIgnore]
        public int TypeId { get; set; }

        [JsonIgnore]
        public long TrackingId { get; set; }
    }
}
