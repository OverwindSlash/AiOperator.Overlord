using OpenCvSharp;
using Overlord.Core.DataStructures;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Overlord.Domain.EventAlg
{
    public class SlowVehicleEventAlg : EventAlgorithmBase, IObserver<ObjectExpiredEvent>
    {
        private readonly int _fps;
        private readonly ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>> _toiHistory;

        // laneIndex -> array of TimeStamp
        private ConcurrentDictionary<int, FixedSizedQueue<long>> _recentSlowEvents;

        // image and video capture base dir.
        private readonly string _slowSnapshotDir;
        private readonly string _slowVideoDir;
        private readonly string _ambleSnapshotDir;
        private readonly string _ambleVideoDir;

        private int _slowVehicleSpeedUpperLimit;
        private int _slowVehicleSpeedLowerLimit;
        private int _slowVehicleEnableDurationSec;
        private int _minSlowEventsToJudgeAmble;
        private int _ambleJudgeDurationSec;

        public SlowVehicleEventAlg(string captureRoot, int fps, EventProcessor eventProcessor, EventPublisher eventPublisher)
            : base(captureRoot, eventProcessor, eventPublisher)
        {
            _fps = fps;
            _toiHistory = new ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>>();

            _slowSnapshotDir = Path.Combine(_captureRoot, "Snapshot", "Slow");
            _slowVideoDir = Path.Combine(_captureRoot, "Video", "Slow");

            if (!Directory.Exists(_slowSnapshotDir))
            {
                Directory.CreateDirectory(_slowSnapshotDir);
            }

            if (!Directory.Exists(_slowVideoDir))
            {
                Directory.CreateDirectory(_slowVideoDir);
            }

            _ambleSnapshotDir = Path.Combine(_captureRoot, "Snapshot", "Amble");
            _ambleVideoDir = Path.Combine(_captureRoot, "Video", "Amble");

            if (!Directory.Exists(_ambleSnapshotDir))
            {
                Directory.CreateDirectory(_ambleSnapshotDir);
            }

            if (!Directory.Exists(_ambleVideoDir))
            {
                Directory.CreateDirectory(_ambleVideoDir);
            }
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _slowVehicleSpeedUpperLimit = roadDefinition.SlowVehicleSpeedUpperLimit;
            _slowVehicleSpeedLowerLimit = roadDefinition.SlowVehicleSpeedLowerLimit;
            _slowVehicleEnableDurationSec = roadDefinition.SlowVehicleEnableDurationSec;
            _minSlowEventsToJudgeAmble = roadDefinition.MinSlowEventsToJudgeAmble;
            _ambleJudgeDurationSec = roadDefinition.AmbleJudgeDurationSec;

            _recentSlowEvents = new ConcurrentDictionary<int, FixedSizedQueue<long>>();
        }

        public override void DetectEvent(TrafficObjectInfo toi, FrameInfo frameInfo)
        {
            if (!toi.IsAnalyzable)
            {
                return;
            }

            if (!toi.IsMotionCalculated)
            {
                return;
            }

            if (!_toiHistory.ContainsKey(toi.Id))
            {
                _toiHistory.TryAdd(toi.Id,
                    new FixedSizedQueue<TrafficObjectInfo>(_slowVehicleEnableDurationSec * _fps,
                        toi => toi.MotionInfo.Speed <= _slowVehicleSpeedUpperLimit && toi.MotionInfo.Speed >= _slowVehicleSpeedLowerLimit));
            }

            var queue = _toiHistory[toi.Id];
            queue.Enqueue(toi);

            if (queue.IsPositive())
            {
                toi.InStatusSlowSpeed = true;

                Task.Run(async () =>
                {
                    if (_eventProcessor.IsEventNeedTrigger($"S_{toi.Id}"))
                    {
                        toi.EventSlowVehicleRaised = true;

                        await Task.Run(async () =>
                        {
                            await JudgeAndReportAmble(toi);
                        });

                        string timestamp = DateTime.Now.ToString(TimestampPattern);
                        string normalizedFilename = toi.Id.Replace(":", "_");

                        // take snapshot
                        Mat eventSceneImage = _snapshotService.GetSceneByByFrameId(toi.FrameId);
                        string snapshotFile = Path.Combine(_slowSnapshotDir, $"{timestamp}_{normalizedFilename}.jpg");
                        eventSceneImage.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red, 1);
                        eventSceneImage.SaveImage(snapshotFile);

                        // save video
                        string videoFile = Path.Combine(_slowVideoDir, $"{timestamp}_{normalizedFilename}.mp4");
                        _snapshotService.GenerateSnapVideo(videoFile);

                        // report event
                        TrafficEvent slowVehicleEvent = _eventProcessor.CreateSlowVehicleEvent(_roadDefinition.DeviceNo, toi.LaneIndex, toi.TypeId, toi.TrackingId);
                        slowVehicleEvent.EventCategory = "Slow";
                        slowVehicleEvent.LocalImageFilePath = snapshotFile;
                        slowVehicleEvent.LocalVideoFilePath = videoFile;
                        bool result = await _eventPublisher.Publish(slowVehicleEvent);
                    }
                });
            }
        }

        private async Task JudgeAndReportAmble(TrafficObjectInfo toi)
        {
            long ticks = DateTime.Now.Ticks;

            if (!_recentSlowEvents.ContainsKey(toi.LaneIndex))
            {
                _recentSlowEvents.TryAdd(toi.LaneIndex, new FixedSizedQueue<long>(_minSlowEventsToJudgeAmble));
            }

            FixedSizedQueue<long> recentSlowEventsById = _recentSlowEvents[toi.LaneIndex];
            recentSlowEventsById.Enqueue(ticks);

            if ((recentSlowEventsById.Count() == _minSlowEventsToJudgeAmble) && (recentSlowEventsById.Peek(out var fist)))
            {
                long elapseSeconds = (ticks - fist) / 10000000;
                if (elapseSeconds < _ambleJudgeDurationSec)
                {
                    if (_eventProcessor.IsEventNeedTrigger($"Amble_{toi.LaneIndex}"))
                    {
                        toi.EventRoadAmbleRaised = true;

                        string timestamp = DateTime.Now.ToString(TimestampPattern);
                        string normalizedFilename = $"Lane_{toi.LaneIndex}";

                        // take snapshot
                        Mat eventSceneImage = _snapshotService.GetSceneByByFrameId(toi.FrameId);
                        string snapshotFile = Path.Combine(_ambleSnapshotDir, $"{timestamp}_{normalizedFilename}.jpg");
                        eventSceneImage.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red, 1);
                        eventSceneImage.SaveImage(snapshotFile);

                        // save video
                        string videoFile = Path.Combine(_ambleVideoDir, $"{timestamp}_{normalizedFilename}.mp4");
                        _snapshotService.GenerateSnapVideo(videoFile);

                        // report event
                        TrafficEvent roadAmbleEvent = _eventProcessor.CreateRoadAmbleEvent(_roadDefinition.DeviceNo, toi.LaneIndex, toi.TypeId, toi.TrackingId);
                        roadAmbleEvent.EventCategory = "Amble";
                        roadAmbleEvent.LocalImageFilePath = snapshotFile;
                        roadAmbleEvent.LocalVideoFilePath = videoFile;
                        bool result = await _eventPublisher.Publish(roadAmbleEvent);
                    }
                }
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
                ReleaseToiHistoryById(value.Id);
            });
        }

        private void ReleaseToiHistoryById(string id)
        {
            if (_toiHistory.Keys.Contains(id))
            {
                _toiHistory.TryRemove(id, out var value);
            }
        }
    }
}
