using Overlord.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Overlord.Domain.Event
{
    public class EventProcessor : IObserver<ObjectExpiredEvent>
    {
        private readonly int _minTriggerIntervalSecs;
        private readonly ConcurrentDictionary<string, DateTime> _lastEventTime;

        private readonly ITrafficEventGenerator _trafficEventGenerator;

        public ConcurrentDictionary<string, DateTime> LastEventTime => _lastEventTime;

        public EventProcessor(int minTriggerIntervalSecs, ITrafficEventGenerator trafficEventGenerator)
        {
            _minTriggerIntervalSecs = minTriggerIntervalSecs;
            _lastEventTime = new ConcurrentDictionary<string, DateTime>();

            _trafficEventGenerator = trafficEventGenerator;
        }

        public bool IsEventNeedTrigger(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return false;
            }

            DateTime now = DateTime.Now;

            // first trigger
            if (!_lastEventTime.ContainsKey(eventId))
            {
                _lastEventTime.TryAdd(eventId, now);
                return true;
            }

            // non-first trigger
            DateTime lastTriggerTime = _lastEventTime[eventId];
            TimeSpan span = now - lastTriggerTime;
            if (span.TotalSeconds > _minTriggerIntervalSecs)
            {
                // exceed min trigger time interval, update event timestamp and trigger event.
                _lastEventTime[eventId] = now;
                return true;
            }
            else
            {
                // not exceed min trigger time interval, prevent repeat trigger.
                return false;
            }
        }

        public TrafficEvent CreateForbiddenTypeEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            return _trafficEventGenerator.CreateForbiddenEvent(deviceNo, laneIndex, typeId, trackingId);
        }

        public TrafficEvent CreateStoppedVehicleEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            return _trafficEventGenerator.CreateStoppedEvent(deviceNo, laneIndex, typeId, trackingId);
        }

        public TrafficEvent CreateSlowVehicleEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            return _trafficEventGenerator.CreateSlowEvent(deviceNo, laneIndex, typeId, trackingId);
        }

        public TrafficEvent CreateRoadAmbleEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            return _trafficEventGenerator.CreateAmbleEvent(deviceNo, laneIndex, typeId, trackingId);
        }

        public TrafficEvent CreateRoadJamEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            return _trafficEventGenerator.CreateJamEvent(deviceNo, laneIndex, typeId, trackingId);
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
                ReleaseLastEventTimeById(value.Id);
            });
        }

        private void ReleaseLastEventTimeById(string id)
        {
            string forbiddenEventId = $"F_{id}";
            _lastEventTime.TryRemove(forbiddenEventId, out var value1);

            string stopEventId = $"S_{id}";
            _lastEventTime.TryRemove(stopEventId, out var value2);
        }
    }
}
