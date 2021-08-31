using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using System;

namespace Overlord.Domain.Handlers
{
    public interface IAnalysisHandler : IDisposable
    {
        void SetRoadDefinition(RoadDefinition roadDefinition);
        FrameInfo Analyze(FrameInfo frameInfo);
    }
}
