using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Interfaces;
using System;

namespace Overlord.Domain.Handlers
{
    public class DetectionHandler : AnalysisHandlerBase
    {
        private readonly IObjectDetector _detector;

        public DetectionHandler(IObjectDetector detector)
        {
            _detector = detector ?? throw new ArgumentException("detector not initialized.");
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _detector.SetTrackingParam(roadDefinition.TrackingChangeHistory, roadDefinition.TrackingFramesStory, roadDefinition.TrackingMaxDistance);
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            frameInfo.ObjectInfos = _detector.Detect(frameInfo.Scene, _roadDefinition.DetectionThresh);
            return frameInfo;
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _detector?.Dispose();
            }
        }

        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DetectionHandler()
        {
            Dispose(false);
        }
    }
}
