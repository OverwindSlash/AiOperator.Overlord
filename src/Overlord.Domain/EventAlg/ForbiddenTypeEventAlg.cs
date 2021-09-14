using OpenCvSharp;
using Overlord.Core.DataStructures;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Overlord.Domain.EventAlg
{
    public class ForbiddenTypeEventAlg : EventAlgorithmBase, IObserver<ObjectExpiredEvent>
    {
        private readonly ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>> _driveLaneToiHistory;
        private readonly ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>> _emergencyLaneToiHistory;

        private int _driveLaneForbiddenDurationFrame;
        private int _emergencyLaneForbiddenDurationFrame;

        // image and video capture base dir.
        private readonly string _snapshotDir;
        private readonly string _videoDir;

        // for lookup efficiency
        private Dictionary<int, Lane> _lanes;

        public ForbiddenTypeEventAlg(string captureRoot, EventProcessor eventProcessor, EventPublisher eventPublisher)
            : base(captureRoot, eventProcessor, eventPublisher)
        {
            _driveLaneToiHistory = new ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>>();
            _emergencyLaneToiHistory = new ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>>();
           
            _snapshotDir = Path.Combine(_captureRoot, "Snapshot", "Forbidden");
            _videoDir = Path.Combine(_captureRoot, "Video", "Forbidden");

            if (!Directory.Exists(_snapshotDir))
            {
                Directory.CreateDirectory(_snapshotDir);
            }

            if (!Directory.Exists(_videoDir))
            {
                Directory.CreateDirectory(_videoDir);
            }
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _driveLaneForbiddenDurationFrame = roadDefinition.DriveLaneForbiddenDurationFrame;
            _emergencyLaneForbiddenDurationFrame = roadDefinition.EmergencyLaneForbiddenDurationFrame;

            // for lookup efficiency
            _lanes = new Dictionary<int, Lane>();
            foreach (Lane lane in roadDefinition.Lanes)
            {
                _lanes.Add(lane.Index, lane);
            }
        }

        public override void DetectEvent(TrafficObjectInfo toi, FrameInfo frameInfo)
        {
            if (!toi.IsAnalyzable)
            {
                return;
            }

            if (!_lanes.ContainsKey(toi.LaneIndex))
            {
                return;
            }

            Lane lane = _lanes[toi.LaneIndex];
            if (lane.ForbiddenTypes.Contains(toi.Type))
            {
                if (IsInSpecialCases(toi, frameInfo))
                {
                    return;
                }

                toi.InEventEnterForbiddenRegion = true;

                bool isPositive = false;
                if (lane.Type == LaneType.DriveLane)
                {
                    if (!_driveLaneToiHistory.ContainsKey(toi.Id))
                    {
                        _driveLaneToiHistory.TryAdd(toi.Id, new FixedSizedQueue<TrafficObjectInfo>(
                            _driveLaneForbiddenDurationFrame, item => item.InEventEnterForbiddenRegion));
                    }

                    FixedSizedQueue<TrafficObjectInfo> driveLaneHistory = _driveLaneToiHistory[toi.Id];
                    driveLaneHistory.Enqueue(toi);
                    isPositive = driveLaneHistory.IsPositive();
                }

                if (lane.Type == LaneType.EmergencyLane)
                {
                    if (!_emergencyLaneToiHistory.ContainsKey(toi.Id))
                    {
                        _emergencyLaneToiHistory.TryAdd(toi.Id, new FixedSizedQueue<TrafficObjectInfo>(
                            _emergencyLaneForbiddenDurationFrame, item => item.InEventEnterForbiddenRegion));
                    }

                    FixedSizedQueue<TrafficObjectInfo> emergencyLaneHistory = _emergencyLaneToiHistory[toi.Id];
                    emergencyLaneHistory.Enqueue(toi);
                    isPositive = emergencyLaneHistory.IsPositive();
                }

                if (isPositive)
                {
                    Task.Run(async () =>
                    {
                        if (_eventProcessor.IsEventNeedTrigger($"F_{toi.Id}"))
                        {
                            string timestamp = DateTime.Now.ToString(TimestampPattern);
                            string normalizedFilename = toi.Id.Replace(":", "_");

                            // take snapshot
                            Mat eventSceneImage = _snapshotService.GetSceneByByFrameId(toi.FrameId);
                            string snapshotFile = Path.Combine(_snapshotDir, $"{timestamp}_{normalizedFilename}.jpg");
                            eventSceneImage.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red, 1);
                            eventSceneImage.SaveImage(snapshotFile);

                            // save video
                            string videoFile = Path.Combine(_videoDir, $"{timestamp}_{normalizedFilename}.mp4");
                            _snapshotService.GenerateSnapVideo(videoFile);

                            // report event
                            TrafficEvent forbiddenEvent = _eventProcessor.CreateForbiddenEvent(_roadDefinition.DeviceNo, toi.TypeId, toi.TrackingId);
                            forbiddenEvent.EventCategory = "Forbidden";
                            bool result = await _eventPublisher.Publish(forbiddenEvent);
                        }
                    });
                }
            }
        }

        private static bool IsInSpecialCases(TrafficObjectInfo toi, FrameInfo frameInfo)
        {
            // exclude person in vehicle
            foreach (TrafficObjectInfo otherToi in frameInfo.ObjectInfos)
            {
                if (otherToi.IsContainObject(toi))
                {
                    return true;
                }
            }

            // to avoid mis-detection some part of vehicle as person
            if (toi.Type.ToLower() == "person")
            {
                if (toi.MotionInfo.Speed > 10.0)
                {
                    return true;
                }
            }

            return false;
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
                ReleaseLaneToiHistoryById(value.Id);
            });
        }

        private void ReleaseLaneToiHistoryById(string id)
        {
            if (_driveLaneToiHistory.Keys.Contains(id))
            {
                _driveLaneToiHistory.TryRemove(id, out var value);
            }

            if (_emergencyLaneToiHistory.Keys.Contains(id))
            {
                _emergencyLaneToiHistory.TryRemove(id, out var value);
            }
        }
    }
}