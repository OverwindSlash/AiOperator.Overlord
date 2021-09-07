using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using Overlord.Domain.Services;
using System;

namespace Overlord.Domain.EventAlg
{
    public abstract class EventAlgorithmBase : IEventAlgorithm
    {
        protected const string TimestampPattern = "yyyyMMddHHmmss";

        protected readonly string _captureRoot;
        protected readonly EventProcessor _eventProcessor;
        protected readonly EventPublisher _eventPublisher;

        protected RoadDefinition _roadDefinition;
        protected SnapshotService _snapshotService;

        protected EventAlgorithmBase(string captureRoot, EventProcessor eventProcessor, EventPublisher eventPublisher)
        {
            _captureRoot = captureRoot;
            _eventProcessor = eventProcessor;
            _eventPublisher = eventPublisher;
        }

        public virtual void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
        }

        public void SetSnapshotService(SnapshotService snapshotService)
        {
            _snapshotService = snapshotService;
        }

        public abstract void DetectEvent(TrafficObjectInfo toi, FrameInfo frameInfo);


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do nothing
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
