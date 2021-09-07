using Overlord.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Overlord.Domain.Event
{
    public class EventProcessor
    {
        private readonly int _minTriggerIntervalSecs = 30;
        private readonly Dictionary<string, DateTime> _lastEventTime;

        private readonly ITrafficEventGenerator _trafficEventGenerator;

        public EventProcessor(int minTriggerIntervalSecs, ITrafficEventGenerator trafficEventGenerator)
        {
            _minTriggerIntervalSecs = minTriggerIntervalSecs;
            _lastEventTime = new Dictionary<string, DateTime>();

            _trafficEventGenerator = trafficEventGenerator;
        }

        public bool IsEventNeedTrigger(string eventId)
        {
            DateTime now = DateTime.Now;

            // first trigger
            if (!_lastEventTime.ContainsKey(eventId))
            {
                _lastEventTime.Add(eventId, now);
                return true;
            }

            // non-first trigger
            DateTime lastTriggerTime = _lastEventTime[eventId];
            TimeSpan span = now - lastTriggerTime;
            if (span.TotalSeconds > _minTriggerIntervalSecs)
            {
                // exceed min trigger time interval, trigger event.
                _lastEventTime[eventId] = now;
                return true;
            }
            else
            {
                // not exceed min trigger time interval, prevent repeat trigger.
                return false;
            }
        }

        public TrafficEvent CreateForbiddenEvent(string deviceNo, int typeId, long trackingId)
        {
            return _trafficEventGenerator.CreateForbiddenEvent(deviceNo, typeId, trackingId);
        }
    }
}
