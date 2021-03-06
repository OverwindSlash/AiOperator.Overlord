using System.Text.Json.Serialization;

namespace Overlord.Domain.Event
{
    public class TrafficEvent : EventBase
    {
        [JsonIgnore]
        public string DeviceNo { get; set; }
        
        [JsonIgnore]
        public int LaneIndex { get; set; }

        [JsonIgnore]
        public string EventCategory { get; set; }

        [JsonIgnore]
        public int TypeId { get; set; }

        [JsonIgnore]
        public long TrackingId { get; set; }
        
        [JsonIgnore]
        public string LocalImageFilePath { get; set; }
        
        [JsonIgnore]
        public string LocalVideoFilePath { get; set; }
    }
}
