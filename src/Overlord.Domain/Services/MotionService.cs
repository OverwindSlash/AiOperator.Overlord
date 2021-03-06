using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using Overlord.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overlord.Domain.Services
{
    public class MotionService : ObserverBase<ObjectExpiredEvent>
    {
        private RoadDefinition _roadDefinition;
        private int _motionCalculationFrameInterval;

        private readonly ISpeeder _speeder;

        // object motion history. objectId -> (frameId, toi)
        private readonly ConcurrentDictionary<string, SortedList<long, TrafficObjectInfo>> _motionHistory;

        public MotionService(ISpeeder speeder)
        {
            _motionHistory = new ConcurrentDictionary<string, SortedList<long, TrafficObjectInfo>>();
            _speeder = speeder;
        }

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
            _motionCalculationFrameInterval = _roadDefinition.MotionCalculationFrameInterval;

            if (_motionCalculationFrameInterval < 1)
            {
                throw new ArgumentException("road definition file not correct.");
            }

            if (_speeder != null)
            {
                _speeder.SetRoadDefinition(roadDefinition);
            }
        }

        public void AddTrafficObjectInfoHistory(long frameId, TrafficObjectInfo toi)
        {
            if (!toi.IsAnalyzable)
            {
                return;
            }

            if (!_motionHistory.Keys.Contains(toi.Id))
            {
                _motionHistory[toi.Id] = new SortedList<long, TrafficObjectInfo>();
            }

            SortedList<long, TrafficObjectInfo> objectToiHistory = _motionHistory[toi.Id];
            if (!objectToiHistory.ContainsKey(frameId))
            {
                objectToiHistory.Add(frameId, toi);
            }
            else
            {
                objectToiHistory[frameId] = toi;
            }

            CalculateMotion(objectToiHistory);
        }

        private void CalculateMotion(SortedList<long, TrafficObjectInfo> objectToiHistory)
        {
            int historyCount = objectToiHistory.Values.Count;

            TrafficObjectInfo currentToi = objectToiHistory.Values.Last();
            MotionInfo currentMotionInfo = currentToi.MotionInfo;

            // motion calculation will be available when history count is bigger than _motionCalculationFrameInterval
            if (historyCount > _motionCalculationFrameInterval)
            {
                // record motion information of previous internal in current TrafficObjectInfo object.
                TrafficObjectInfo lastToi = objectToiHistory.Values[(historyCount - 1) - _motionCalculationFrameInterval];
                MotionInfo lastMotionInfo = lastToi.MotionInfo;

                currentMotionInfo.PrevIntervalToiFrameId = lastToi.FrameId;
                currentMotionInfo.PrevIntervalToiTimespan = currentToi.TimeStamp - lastToi.TimeStamp;

                currentMotionInfo.XOffset = currentToi.CenterX - lastToi.CenterX;
                currentMotionInfo.YOffset = currentToi.CenterY - lastToi.CenterY;

                currentMotionInfo.Offset = Math.Sqrt(currentMotionInfo.XOffset * currentMotionInfo.XOffset + currentMotionInfo.YOffset * currentMotionInfo.YOffset);

                if (_speeder != null)
                {
                    currentMotionInfo.Distance = _speeder.CalculateDistance(currentToi);
                    currentMotionInfo.Speed = currentMotionInfo.Distance / currentMotionInfo.PrevIntervalToiTimespan.TotalHours;
                    currentMotionInfo.Direction = _speeder.CalculateDirection(currentToi);
                    currentMotionInfo.IsSpeedCalculated = true;
                }
            }
        }

        public SortedList<long, TrafficObjectInfo> GetMotionHistoryById(string objectId)
        {
            if (_motionHistory.ContainsKey(objectId))
            {
                return _motionHistory[objectId];
            }

            return new SortedList<long, TrafficObjectInfo>();
        }

        
        public override void OnNext(ObjectExpiredEvent value)
        {
            Task.Run(() =>
            {
                ReleaseMotionHistoryById(value.Id);
            });
        }

        private void ReleaseMotionHistoryById(string id)
        {
            if (!_motionHistory.Keys.Contains(id))
            {
                return;
            }

            _motionHistory.TryRemove(id, out var value);
        }
    }
}
