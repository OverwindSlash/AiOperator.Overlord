﻿using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Services;

namespace Overlord.Domain.Handlers
{
    public class MotionHandler : AnalysisHandlerBase
    {
        private readonly MotionService _motionService;

        public MotionService Service => _motionService;

        public MotionHandler()
        {
            _motionService = new MotionService();
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _motionService.SetRoadDefinition(roadDefinition);
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
            {
                _motionService.AddTrafficObjectInfoHistory(frameInfo.FrameId, toi);
            }

            return frameInfo;
        }
    }
}
