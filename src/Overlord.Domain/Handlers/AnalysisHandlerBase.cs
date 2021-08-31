using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using System;

namespace Overlord.Domain.Handlers
{
    public class AnalysisHandlerBase : IAnalysisHandler
    {
        protected RoadDefinition _roadDefinition;

        public virtual void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition ?? throw new ArgumentException("road definition not initialized.");
        }

        public virtual FrameInfo Analyze(FrameInfo frameInfo)
        {
            return frameInfo;
        }

        public virtual void Dispose()
        {
            // Do nothing.
        }
    }
}
