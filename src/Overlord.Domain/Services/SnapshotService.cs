using OpenCvSharp;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;

namespace Overlord.Domain.Services
{
    public class SnapshotService : IObserver<ObjectExpiredEvent>, IObserver<FrameExpiredEvent>, IDisposable
    {
        // frameId -> Scene
        private readonly ConcurrentDictionary<long, Mat> _scenesOfFrame;

        // objectId -> (confidence, objectMat)
        private readonly ConcurrentDictionary<string, SortedList<float, Mat>> _snapshotsOfObject;

        private RoadDefinition _roadDefinition;
        private int _maxObjectSnapshots;

        public SnapshotService()
        {
            _scenesOfFrame = new ConcurrentDictionary<long, Mat>();
            _snapshotsOfObject = new ConcurrentDictionary<string, SortedList<float, Mat>>();
        }

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
            _maxObjectSnapshots = _roadDefinition.MaxObjectSnapshots;

            if (_maxObjectSnapshots < 1)
            {
                throw new ArgumentException("road definition file not correct.");
            }
        }

        public void AddSceneByFrameId(long frameId, Mat sceneImage)
        {
            if (!_scenesOfFrame.ContainsKey(frameId))
            {
                _scenesOfFrame.TryAdd(frameId, sceneImage);
            }
        }

        public void AddSnapshotOfObjectById(FrameInfo frameInfo)
        {
            foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
            {
                if ((toi.X + toi.Width) > toi.Width || (toi.Y + toi.Height > toi.Height))
                {
                    continue;
                }

                Mat objSnapshot = frameInfo.Scene.SubMat(new Rect(toi.X, toi.Y, toi.Width, toi.Height));
                AddSnapshotOfObjectById(toi.Id, toi.Confidence, objSnapshot);
            }
        }

        private void AddSnapshotOfObjectById(string id, float confidence, Mat snapshot)
        {
            if (!_snapshotsOfObject.ContainsKey(id))
            {
                _snapshotsOfObject.TryAdd(id, new SortedList<float, Mat>());
            }

            SortedList<float, Mat> snapshotsById = _snapshotsOfObject[id];

            if (!snapshotsById.ContainsKey(confidence))
            {
                snapshotsById.Add(confidence, snapshot);
            }
            else
            {
                snapshotsById[confidence] = snapshot;
            }

            if (snapshotsById.Count > _maxObjectSnapshots)
            {
                for (int i = 0; i < snapshotsById.Count - _maxObjectSnapshots; i++)
                {
                    snapshotsById.RemoveAt(_maxObjectSnapshots + i);
                }
            }
        }

        public int GetCacheSceneCount()
        {
            return _scenesOfFrame.Count;
        }

        public Mat GetSceneByByFrameId(long frameId)
        {
            if (_scenesOfFrame.ContainsKey(frameId))
            {
                _scenesOfFrame.TryGetValue(frameId, out var scene);
                return scene;
            }

            return new Mat();
        }

        public void OnCompleted()
        {
            // Do nothing
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public void OnNext(FrameExpiredEvent value)
        {
            Task.Run(() =>
            {
                ReleaseSceneByFrameId(value.FrameId);
            });
        }

        private void ReleaseSceneByFrameId(long frameId)
        {
            if (_scenesOfFrame.ContainsKey(frameId))
            {
                _scenesOfFrame[frameId].Dispose();

                _scenesOfFrame.TryRemove(frameId, out var mat);
            }
        }

        public void OnNext(ObjectExpiredEvent value)
        {
            Task.Run(() =>
            {
                ReleaseSnapshotsByObjectId(value.Id);
            });
        }

        private void ReleaseSnapshotsByObjectId(string id)
        {
            if (!_snapshotsOfObject.ContainsKey(id))
            {
                return;
            }

            SortedList<float, Mat> snapshots = _snapshotsOfObject[id];
            foreach (Mat snapshot in snapshots.Values)
            {
                snapshot.Dispose();
            }

            _snapshotsOfObject.TryRemove(id, out var removedSnapshots);
        }

        public void Dispose()
        {
            foreach (Mat scene in _scenesOfFrame.Values)
            {
                scene.Dispose();
            }
        }
    }
}
