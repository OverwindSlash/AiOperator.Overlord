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

        // image and video capture base dir.
        private readonly string _snapshotDir;
        private readonly string _videoDir;

        private int _stopEvnetSpeedUpperLimit;
        private int _stopEvnetEnableDurationSec;

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

            _snapshotDir = Path.Combine(_captureRoot, "Snapshot", "Stopped");
            _videoDir = Path.Combine(_captureRoot, "Video", "Stopped");

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
            _stopEvnetSpeedUpperLimit = roadDefinition.StopEventSpeedUpperLimit;
            _stopEvnetEnableDurationSec = roadDefinition.StopEventEnableDurationSec;
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
                toi.InEventStopped = true;

                Task.Run(async () =>
                {
                    if (_eventProcessor.IsEventNeedTrigger($"P_{toi.Id}"))
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
                        TrafficEvent stoppedEvent = _eventProcessor.CreateStoppedEvent(_roadDefinition.DeviceNo, toi.LaneIndex, toi.TypeId, toi.TrackingId);
                        stoppedEvent.EventCategory = "Stopped";
                        stoppedEvent.LocalImageFilePath = snapshotFile;
                        stoppedEvent.LocalVideoFilePath = videoFile;
                        bool result = await _eventPublisher.Publish(stoppedEvent);
                    }
                });
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
