using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Overlord.Core.DataStructures;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;

namespace Overlord.Domain.EventAlg
{
    public class SlowVehicleEventAlg : EventAlgorithmBase, IObserver<ObjectExpiredEvent>
    {
        private readonly int _fps;
        private readonly ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>> _toiHistory;

        // image and video capture base dir.
        private readonly string _snapshotDir;
        private readonly string _videoDir;

        private int _slowVehicleSpeedUpperLimit;
        private int _slowVehicleSpeedLowerLimit;
        private int _slowVehicleEnableDurationSec;

        public SlowVehicleEventAlg(string captureRoot, int fps, EventProcessor eventProcessor, EventPublisher eventPublisher)
            : base(captureRoot, eventProcessor, eventPublisher)
        {
            _toiHistory = new ConcurrentDictionary<string, FixedSizedQueue<TrafficObjectInfo>>();
            _fps = fps;

            _snapshotDir = Path.Combine(_captureRoot, "Snapshot", "Slow");
            _videoDir = Path.Combine(_captureRoot, "Video", "Slow");

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
            _slowVehicleSpeedUpperLimit = roadDefinition.SlowVehicleSpeedUpperLimit;
            _slowVehicleSpeedLowerLimit = roadDefinition.SlowVehicleSpeedLowerLimit;
            _slowVehicleEnableDurationSec = roadDefinition.SlowVehicleEnableDurationSec;
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
                toi.InEventSlowSpeed = true;

                Task.Run(async () =>
                {
                    if (_eventProcessor.IsEventNeedTrigger($"S_{toi.Id}"))
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
                        TrafficEvent slowVehicleEvent = _eventProcessor.CreateSlowVehicleEvent(_roadDefinition.DeviceNo, toi.LaneIndex, toi.TypeId, toi.TrackingId);
                        slowVehicleEvent.EventCategory = "Slow";
                        slowVehicleEvent.LocalImageFilePath = snapshotFile;
                        slowVehicleEvent.LocalVideoFilePath = videoFile;
                        bool result = await _eventPublisher.Publish(slowVehicleEvent);
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
