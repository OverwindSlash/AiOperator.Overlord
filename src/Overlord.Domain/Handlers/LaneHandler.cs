using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Services;

namespace Overlord.Domain.Handlers
{
    public class LaneHandler : AnalysisHandlerBase
    {
        private LaneService _laneService;

        public LaneHandler()
        {
            _laneService = new LaneService();
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _laneService.SetRoadDefinition(roadDefinition);
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            foreach (TrafficObjectInfo toi in frameInfo.TrafficObjectInfos)
            {
                toi.LaneIndex = _laneService.CalculateLane(toi);
            }
            
            return frameInfo;
        }
    }
}
