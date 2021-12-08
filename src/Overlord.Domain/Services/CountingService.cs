using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Geometric;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Overlord.Domain.Services
{
    public class CountingService : ObserverBase<ObjectExpiredEvent>
    {
        private RoadDefinition _roadDefinition;
        private List<Tuple<EnterLine, LeaveLine>> _countLines;
        private bool _isDoubleLineCounting;

        private readonly List<EnterLine> _enterLines;
        private readonly List<LeaveLine> _leaveLines;

        // objectId -> count line index
        private readonly ConcurrentDictionary<string, int> _objsCrossedEnterLine;
        private readonly ConcurrentDictionary<string, int> _objsCrossedLeaveLine;

        // to avoid repeat count, objectId -> frameId
        private readonly ConcurrentDictionary<string, long> _objsCounted;

        // lane type counting, landId -> (objectType -> count)
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _laneObjectTypeCount;


        public ConcurrentDictionary<string, int> ObjsCrossedEnterLine => _objsCrossedEnterLine;

        public ConcurrentDictionary<string, int> ObjsCrossedLeaveLine => _objsCrossedLeaveLine;

        public ConcurrentDictionary<string, long> ObjsCounted => _objsCounted;


        public CountingService()
        {
            _enterLines = new List<EnterLine>();
            _leaveLines = new List<LeaveLine>();

            _objsCrossedEnterLine = new ConcurrentDictionary<string, int>();
            _objsCrossedLeaveLine = new ConcurrentDictionary<string, int>();
            _objsCounted = new ConcurrentDictionary<string, long>();

            _laneObjectTypeCount = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
        }

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
            _countLines = _roadDefinition.CountLines;
            _isDoubleLineCounting = roadDefinition.IsDoubleLineCounting;

            _objsCrossedEnterLine.Clear();
            _objsCrossedLeaveLine.Clear();
            _objsCounted.Clear();

            foreach (ConcurrentDictionary<string, int> typeCount in _laneObjectTypeCount.Values)
            {
                typeCount.Clear();
            }
            _laneObjectTypeCount.Clear();

            _enterLines.Clear();
            _leaveLines.Clear();

            foreach (Tuple<EnterLine, LeaveLine> countLine in _countLines)
            {
                _enterLines.Add(countLine.Item1);
                _leaveLines.Add(countLine.Item2);
            }
        }

        public void CountTrafficeObject(TrafficObjectInfo toi)
        {
            // avoid repeat counting
            if (_objsCounted.ContainsKey(toi.Id))
            {
                toi.WasCounted = true;
                return;
            }

            // no movement, skip counting
            if ((toi.MotionInfo.XOffset == 0) && (toi.MotionInfo.YOffset == 0))
            {
                return;
            }

            NormalizedPoint currentPos = new NormalizedPoint(_roadDefinition.ImageWidth, _roadDefinition.ImageHeight,
                toi.CenterX, toi.CenterY);

            NormalizedPoint lastPos = new NormalizedPoint(_roadDefinition.ImageWidth, _roadDefinition.ImageHeight,
                toi.CenterX - toi.MotionInfo.XOffset, toi.CenterY - toi.MotionInfo.YOffset);

            CheckCrossedEnterLines(toi, currentPos, lastPos);

            CheckCrossedLeaveLines(toi, currentPos, lastPos);
            
            bool needCounting = WillNeedCounting(toi);

            if (needCounting)
            {
                UpdateLaneObjectTypeCount(toi);

                UpdateObjectCounted(toi);
            }
        }

        private void CheckCrossedEnterLines(TrafficObjectInfo toi, NormalizedPoint currentPos, NormalizedPoint lastPos)
        {
            if (!_objsCrossedEnterLine.ContainsKey(toi.Id))
            {
                foreach (EnterLine enterLine in _enterLines)
                {
                    if (enterLine.IsCrossedLine(currentPos, lastPos))
                    {
                        _objsCrossedEnterLine.TryAdd(toi.Id, _enterLines.IndexOf(enterLine));
                    }
                }
            }
        }

        private void CheckCrossedLeaveLines(TrafficObjectInfo toi, NormalizedPoint currentPos, NormalizedPoint lastPos)
        {
            if (!_objsCrossedLeaveLine.ContainsKey(toi.Id))
            {
                foreach (LeaveLine leaveLine in _leaveLines)
                {
                    if (leaveLine.IsCrossedLine(currentPos, lastPos))
                    {
                        _objsCrossedLeaveLine.TryAdd(toi.Id, _leaveLines.IndexOf(leaveLine));
                    }
                }
            }
        }

        private bool WillNeedCounting(TrafficObjectInfo toi)
        {
            if (_isDoubleLineCounting)
            {
                if (_objsCrossedEnterLine.ContainsKey(toi.Id) && _objsCrossedLeaveLine.ContainsKey(toi.Id))
                {
                    return true;
                }
            }
            else
            {
                if (_objsCrossedEnterLine.ContainsKey(toi.Id))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateObjectCounted(TrafficObjectInfo toi)
        {
            if (!_objsCounted.ContainsKey(toi.Id))
            {
                _objsCounted.TryAdd(toi.Id, toi.FrameId);
            }
        }

        private void UpdateLaneObjectTypeCount(TrafficObjectInfo toi)
        {
            if (!_laneObjectTypeCount.ContainsKey(toi.LaneIndex))
            {
                _laneObjectTypeCount.TryAdd(toi.LaneIndex, new ConcurrentDictionary<string, int>());
            }

            _laneObjectTypeCount.TryGetValue(toi.LaneIndex, out var typesCountsByLaneId);
            if (!typesCountsByLaneId.ContainsKey(toi.Type))
            {
                typesCountsByLaneId.TryAdd(toi.Type, 0);
            }

            typesCountsByLaneId.TryGetValue(toi.Type, out var typeCounts);
            typeCounts++;
            typesCountsByLaneId.TryUpdate(toi.Type, typeCounts, typeCounts - 1);
        }

        public int GetLaneObjectTypeCount(int laneId, string typeName)
        {
            if (!_laneObjectTypeCount.ContainsKey(laneId))
            {
                return 0;
            }

            _laneObjectTypeCount.TryGetValue(laneId, out var typesCountsByLaneId);
            if (!typesCountsByLaneId.ContainsKey(typeName))
            {
                return 0;
            }

            typesCountsByLaneId.TryGetValue(typeName, out var typeCounts);
            return typeCounts;
        }

        public override void OnNext(ObjectExpiredEvent value)
        {
            Task.Run(() =>
            {
                ReleaseObjectCountedById(value.Id);
            });
        }

        private void ReleaseObjectCountedById(string objectId)
        {
            if (_objsCrossedEnterLine.ContainsKey(objectId))
            {
                _objsCrossedEnterLine.TryRemove(objectId, out var value);
            }

            if (_objsCrossedLeaveLine.ContainsKey(objectId))
            {
                _objsCrossedLeaveLine.TryRemove(objectId, out var value);
            }

            if (_objsCounted.ContainsKey(objectId))
            {
                _objsCounted.TryRemove(objectId, out var value);
            }
        }
    }
}
