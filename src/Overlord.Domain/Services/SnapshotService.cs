using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using OpenCvSharp;

namespace Overlord.Domain.Services
{
    public class SnapshotService : IObserver<ObjectExpiredEvent>
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
    }
}
