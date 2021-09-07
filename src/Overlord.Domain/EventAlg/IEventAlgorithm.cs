using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using System;
using Overlord.Domain.Services;

namespace Overlord.Domain.EventAlg
{
    public interface IEventAlgorithm : IDisposable
    {
        void SetRoadDefinition(RoadDefinition roadDefinition);
        void SetSnapshotService(SnapshotService snapshotService);
        void DetectEvent(TrafficObjectInfo toi, FrameInfo frameInfo);
    }
}
