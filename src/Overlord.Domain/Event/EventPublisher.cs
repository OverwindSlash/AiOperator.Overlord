using Overlord.Domain.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Overlord.Domain.Event
{
    public class EventPublisher
    {
        private readonly ITrafficEventPublisher _publisher;

        private readonly ConcurrentDictionary<string, List<TrafficEvent>> _publishedEventsByCategory;

        public ConcurrentDictionary<string, List<TrafficEvent>> PublishedEventsByCategory => _publishedEventsByCategory;

        public EventPublisher(ITrafficEventPublisher publisher)
        {
            _publisher = publisher;
            _publishedEventsByCategory = new ConcurrentDictionary<string, List<TrafficEvent>>();
        }

        public async Task<bool> Publish(TrafficEvent trafficEvent)
        {
            // TODO: Multi pipeline support
            if (!_publishedEventsByCategory.Keys.Contains(trafficEvent.EventCategory))
            {
                _publishedEventsByCategory.TryAdd(trafficEvent.EventCategory, new List<TrafficEvent>());
            }

            _publishedEventsByCategory.TryGetValue(trafficEvent.EventCategory, out var events);
            if (events != null)
            {
                events.Add(trafficEvent);
            }

            // TODO: Real publish logic
            return false;
        }
    }
}
