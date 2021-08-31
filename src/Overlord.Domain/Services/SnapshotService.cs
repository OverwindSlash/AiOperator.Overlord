using OpenCvSharp;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;

namespace Overlord.Domain.Services
{
    public class SnapshotService : IObserver<ObjectExpiredEvent>, IDisposable
    {
        // frameId -> Scene
        private readonly ConcurrentDictionary<long, Mat> _scenesOfFrame;

        public SnapshotService()
        {
            _scenesOfFrame = new ConcurrentDictionary<long, Mat>();
        }

        public void AddSceneByFrameId(long frameId, Mat sceneImage)
        {
            if (!_scenesOfFrame.ContainsKey(frameId))
            {
                _scenesOfFrame.TryAdd(frameId, sceneImage);
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
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ObjectExpiredEvent value)
        {
            throw new NotImplementedException();
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
