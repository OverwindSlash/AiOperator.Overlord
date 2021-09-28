using Overlord.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Overlord.Domain.Event
{
    public class EventPublisher : IObserver<ObjectExpiredEvent>
    {
        private readonly ITrafficEventPublisher _publisher;

        // objectId -> events
        private readonly ConcurrentDictionary<string, ConcurrentBag<TrafficEvent>> _publishedEventsByCategory;

        public ConcurrentDictionary<string, ConcurrentBag<TrafficEvent>> PublishedEventsByCategory => _publishedEventsByCategory;

        public EventPublisher(ITrafficEventPublisher publisher)
        {
            _publisher = publisher;
            _publishedEventsByCategory = new ConcurrentDictionary<string, ConcurrentBag<TrafficEvent>>();
        }

        public async Task<bool> Publish(TrafficEvent trafficEvent)
        {
            if (!_publishedEventsByCategory.Keys.Contains(trafficEvent.EventCategory))
            {
                _publishedEventsByCategory.TryAdd(trafficEvent.EventCategory, new ConcurrentBag<TrafficEvent>());
            }

            _publishedEventsByCategory.TryGetValue(trafficEvent.EventCategory, out var events);
            if (events != null)
            {
                events.Add(trafficEvent);
            }

            try
            {
                //return await _publisher.ReportEvent(trafficEvent);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void OnCompleted()
        {
            // Do nothing
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public void OnNext(ObjectExpiredEvent value)
        {
            Task.Run(() =>
            {
                ReleasePublishedEventById(value.Id);
            });
        }

        private void ReleasePublishedEventById(string id)
        {
            _publishedEventsByCategory.TryRemove(id, out var value);
        }
    }
}
