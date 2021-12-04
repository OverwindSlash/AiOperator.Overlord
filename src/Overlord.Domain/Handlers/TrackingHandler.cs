using Overlord.Core.Entities.Frame;
using Overlord.Domain.Interfaces;
using System;

namespace Overlord.Domain.Handlers
{
    public class TrackingHandler : AnalysisHandlerBase
    {
        private readonly IMultiObjectTracker _tracker;

        public TrackingHandler(IMultiObjectTracker tracker)
        {
            _tracker = tracker ?? throw new ArgumentException("tracker not initialized.");
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            _tracker.Track(frameInfo.TrafficObjectInfos);
            return frameInfo;
        }
    }
}
