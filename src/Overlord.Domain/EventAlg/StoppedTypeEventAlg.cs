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
    public class StoppedTypeEventAlg : EventAlgorithmBase, IObserver<ObjectExpiredEvent>
    {
        private readonly int _fps;
        private readonly HashSet<string> _suitableTypes;
        private readonly ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>> _toiHistory;

        // laneIndex -> array of TimeStamp
        private ConcurrentDictionary<int, FixedSizedQueue<long>> _recentStopEvents;

        // image and video capture base dir.
        private readonly string _stopSnapshotDir;
        private readonly string _stopVideoDir;
        private readonly string _jamSnapshotDir;
        private readonly string _jamVideoDir;

        private int _stopEvnetSpeedUpperLimit;
        private int _stopEvnetEnableDurationSec;
        private int _minStopEventsToJudgeJam;
        private int _jamJudgeDurationSec;

        public StoppedTypeEventAlg(string captureRoot, int fps, EventProcessor eventProcessor, EventPublisher eventPublisher) 
            : base(captureRoot, eventProcessor, eventPublisher)
        {
            _suitableTypes = new HashSet<string>();
            _suitableTypes.Add("car");
            _suitableTypes.Add("truck");
            _suitableTypes.Add("bus");
            _suitableTypes.Add("bike");
            _suitableTypes.Add("motorbike");
            _suitableTypes.Add("train");

            _toiHistory = new ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>>();
            _fps = fps;

            _stopSnapshotDir = Path.Combine(_captureRoot, "Snapshot", "Stopped");
            _stopVideoDir = Path.Combine(_captureRoot, "Video", "Stopped");

            if (!Directory.Exists(_stopSnapshotDir))
            {
                Directory.CreateDirectory(_stopSnapshotDir);
            }

            if (!Directory.Exists(_stopVideoDir))
            {
                Directory.CreateDirectory(_stopVideoDir);
            }

            _jamSnapshotDir = Path.Combine(_captureRoot, "Snapshot", "Jam");
            _jamVideoDir = Path.Combine(_captureRoot, "Video", "Jam");

            if (!Directory.Exists(_jamSnapshotDir))
            {
                Directory.CreateDirectory(_jamSnapshotDir);
            }

            if (!Directory.Exists(_jamVideoDir))
            {
                Directory.CreateDirectory(_jamVideoDir);
            }
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _stopEvnetSpeedUpperLimit = roadDefinition.StopEventSpeedUpperLimit;
            _stopEvnetEnableDurationSec = roadDefinition.StopEventEnableDurationSec;
            _minStopEventsToJudgeJam = roadDefinition.MinStopEventsToJudgeJam;
            _jamJudgeDurationSec = roadDefinition.JamJudgeDurationSec;

            _recentStopEvents = new ConcurrentDictionary<int, FixedSizedQueue<long>>();
        }

        public override void DetectEvent(TrafficObjectInfo toi, FrameInfo frameInfo)
        {
            if (!toi.IsAnalyzable)
            {
                return;
            }

            if (!_suitableTypes.Contains(toi.Type.ToLower()))
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
                    new FixedSizedQueue<TrafficObjectInfo>(_stopEvnetEnableDurationSec * _fps,
                        toi => toi.MotionInfo.Speed <= _stopEvnetSpeedUpperLimit));
            }

            var queue = _toiHistory[toi.Id];
            queue.Enqueue(toi);

            if (queue.IsPositive())
            {
                toi.InStatusStopped = true;

                Task.Run(async () =>
                {
                    if (_eventProcessor.IsEventNeedTrigger($"P_{toi.Id}"))
                    {
                        toi.EventStoppedVehicleRaised = true;

                        await Task.Run(async () =>
                        {
                            await JudgeAndReportJam(toi);
                        });

                        string timestamp = DateTime.Now.ToString(TimestampPattern);
                        string normalizedFilename = toi.Id.Replace(":", "_");

                        // take snapshot
                        Mat eventSceneImage = _snapshotService.GetSceneByByFrameId(toi.FrameId);
                        string snapshotFile = Path.Combine(_stopSnapshotDir, $"{timestamp}_{normalizedFilename}.jpg");
                        eventSceneImage.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red, 1);
                        eventSceneImage.SaveImage(snapshotFile);

                        // save video
                        string videoFile = Path.Combine(_stopVideoDir, $"{timestamp}_{normalizedFilename}.mp4");
                        _snapshotService.GenerateSnapVideo(videoFile);

                        // report event
                        TrafficEvent stoppedEvent = _eventProcessor.CreateStoppedVehicleEvent(_roadDefinition.DeviceNo, toi.LaneIndex, toi.TypeId, toi.TrackingId);
                        stoppedEvent.EventCategory = "Stopped";
                        stoppedEvent.LocalImageFilePath = snapshotFile;
                        stoppedEvent.LocalVideoFilePath = videoFile;
                        bool result = await _eventPublisher.Publish(stoppedEvent);
                    }
                });
            }
        }

        private async Task JudgeAndReportJam(TrafficObjectInfo toi)
        {
            long ticks = DateTime.Now.Ticks;

            if (!_recentStopEvents.ContainsKey(toi.LaneIndex))
            {
                _recentStopEvents.TryAdd(toi.LaneIndex, new FixedSizedQueue<long>(_minStopEventsToJudgeJam));
            }

            FixedSizedQueue<long> recentStopEventsById = _recentStopEvents[toi.LaneIndex];
            recentStopEventsById.Enqueue(ticks);

            if ((recentStopEventsById.Count() == _minStopEventsToJudgeJam) && (recentStopEventsById.Peek(out var fist)))
            {
                long elapseSeconds = (ticks - fist) / 10000000;
                if (elapseSeconds < _jamJudgeDurationSec)
                {
                    if (_eventProcessor.IsEventNeedTrigger($"Jam_{toi.LaneIndex}"))
                    {
                        toi.EventRoadJamRaised = true;

                        string timestamp = DateTime.Now.ToString(TimestampPattern);
                        string normalizedFilename = $"Lane_{toi.LaneIndex}";

                        // take snapshot
                        Mat eventSceneImage = _snapshotService.GetSceneByByFrameId(toi.FrameId);
                        string snapshotFile = Path.Combine(_jamSnapshotDir, $"{timestamp}_{normalizedFilename}.jpg");
                        eventSceneImage.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red, 1);
                        eventSceneImage.SaveImage(snapshotFile);

                        // save video
                        string videoFile = Path.Combine(_jamVideoDir, $"{timestamp}_{normalizedFilename}.mp4");
                        _snapshotService.GenerateSnapVideo(videoFile);

                        // report event
                        TrafficEvent roadJamEvent = _eventProcessor.CreateRoadJamEvent(_roadDefinition.DeviceNo, toi.LaneIndex, toi.TypeId, toi.TrackingId);
                        roadJamEvent.EventCategory = "Jam";
                        roadJamEvent.LocalImageFilePath = snapshotFile;
                        roadJamEvent.LocalVideoFilePath = videoFile;
                        bool result = await _eventPublisher.Publish(roadJamEvent);
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
                _toiHistory.TryRemove(id, out var value1);
            }
        }
    }
}
