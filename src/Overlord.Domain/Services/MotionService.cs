using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overlord.Domain.Services
{
    public class MotionService : IObserver<ObjectExpiredEvent>
    {
        private RoadDefinition _roadDefinition;
        private int _motionCalculationFrameInterval;

        // objectId -> (frameId, toi)
        private readonly ConcurrentDictionary<string, SortedList<long, TrafficObjectInfo>> _motionHistory;

        public MotionService()
        {
            _motionHistory = new ConcurrentDictionary<string, SortedList<long, TrafficObjectInfo>>();
        }

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
            _motionCalculationFrameInterval = _roadDefinition.MotionCalculationFrameInterval;

            if (_motionCalculationFrameInterval < 1)
            {
                throw new ArgumentException("road definition file not correct.");
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

            if (historyCount >= _motionCalculationFrameInterval)
            {
                TrafficObjectInfo lastToi = objectToiHistory.Values[historyCount - _motionCalculationFrameInterval];
                MotionInfo lastMotionInfo = lastToi.MotionInfo;

                currentMotionInfo.LastToiFrameId = lastToi.FrameId;
                currentMotionInfo.LastToiTimespan = currentToi.TimeStamp - lastToi.TimeStamp;

                currentMotionInfo.XOffset = currentToi.CenterX - lastToi.CenterX;
                currentMotionInfo.YOffset = currentToi.CenterY - lastToi.CenterY;

                currentMotionInfo.Offset = Math.Sqrt(currentMotionInfo.XOffset * currentMotionInfo.XOffset + currentMotionInfo.YOffset * currentMotionInfo.YOffset);
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
