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
    public class CountingService : IObserver<ObjectExpiredEvent>
    {
        private RoadDefinition _roadDefinition;
        private List<Tuple<EnterLine, LeaveLine>> _countLines;
        private bool _isDoubleLineCounting;

        private List<EnterLine> _enterLines;
        private List<LeaveLine> _leaveLines;

        // objectId -> count line index
        private readonly ConcurrentDictionary<string, int> _objsCrossedEnterLine;
        private readonly ConcurrentDictionary<string, int> _objsCrossedLeaveLine;

        // to avoid repeat count, objectId -> frameId
        private readonly ConcurrentDictionary<string, long> _objsCounted;

        // landId -> (objectType -> count)
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _laneObjectTypeCounts;

        public ConcurrentDictionary<string, int> ObjsCrossedEnterLine => _objsCrossedEnterLine;

        public ConcurrentDictionary<string, int> ObjsCrossedLeaveLine => _objsCrossedLeaveLine;

        public ConcurrentDictionary<string, long> ObjsCounted => _objsCounted;

        public CountingService()
        {
            _objsCrossedEnterLine = new ConcurrentDictionary<string, int>();
            _objsCrossedLeaveLine = new ConcurrentDictionary<string, int>();
            _objsCounted = new ConcurrentDictionary<string, long>();
            _laneObjectTypeCounts = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
            _enterLines = new List<EnterLine>();
            _leaveLines = new List<LeaveLine>();
        }

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
            _isDoubleLineCounting = roadDefinition.IsDoubleLineCounting;
            _countLines = _roadDefinition.CountLines;

            _objsCrossedEnterLine.Clear();
            _objsCrossedLeaveLine.Clear();
            _objsCounted.Clear();

            foreach (ConcurrentDictionary<string, int> typeCount in _laneObjectTypeCounts.Values)
            {
                typeCount.Clear();
            }
            _laneObjectTypeCounts.Clear();

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
            if (_objsCounted.ContainsKey(toi.Id))
            {
                toi.WasCounted = true;
                return;
            }

            if ((toi.MotionInfo.XOffset == 0) && (toi.MotionInfo.YOffset == 0))
            {
                return;
            }

            NormalizedPoint currentPos = new NormalizedPoint(_roadDefinition.ImageWidth, _roadDefinition.ImageHeight,
                toi.CenterX, toi.CenterY);

            NormalizedPoint lastPos = new NormalizedPoint(_roadDefinition.ImageWidth, _roadDefinition.ImageHeight,
                toi.CenterX - toi.MotionInfo.XOffset, toi.CenterY - toi.MotionInfo.YOffset);

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

            bool needCounting = false;
            if (_isDoubleLineCounting)
            {
                if (_objsCrossedEnterLine.ContainsKey(toi.Id) && _objsCrossedLeaveLine.ContainsKey(toi.Id))
                {
                    needCounting = true;
                }
            }
            else
            {
                if (_objsCrossedEnterLine.ContainsKey(toi.Id))
                {
                    needCounting = true;
                }
            }

            if (needCounting)
            {
                if (!_laneObjectTypeCounts.ContainsKey(toi.LaneIndex))
                {
                    _laneObjectTypeCounts.TryAdd(toi.LaneIndex, new ConcurrentDictionary<string, int>());
                }

                _laneObjectTypeCounts.TryGetValue(toi.LaneIndex, out var typesCountsByLaneId);
                if (!typesCountsByLaneId.ContainsKey(toi.Type))
                {
                    typesCountsByLaneId.TryAdd(toi.Type, 0);
                }

                typesCountsByLaneId.TryGetValue(toi.Type, out var typeCounts);
                typeCounts++;
                typesCountsByLaneId.TryUpdate(toi.Type, typeCounts, typeCounts - 1);

                if (!_objsCounted.ContainsKey(toi.Id))
                {
                    _objsCounted.TryAdd(toi.Id, toi.FrameId);
                }
            }
        }

        public int GetLaneObjectTypeCount(int laneId, string typeName)
        {
            if (!_laneObjectTypeCounts.ContainsKey(laneId))
            {
                return 0;
            }

            _laneObjectTypeCounts.TryGetValue(laneId, out var typesCountsByLaneId);
            if (!typesCountsByLaneId.ContainsKey(typeName))
            {
                return 0;
            }

            typesCountsByLaneId.TryGetValue(typeName, out var typeCounts);
            return typeCounts;
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
