using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Services;

namespace Overlord.Domain.Handlers
{
    public class RegionHandler : AnalysisHandlerBase
    {
        private readonly RegionService _regionService;

        public RegionService Service => _regionService;

        public RegionHandler()
        {
            _regionService = new RegionService();
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _regionService.SetRoadDefinition(roadDefinition);
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            _regionService.DetermindAnalysisObjects(frameInfo.TrafficObjectInfos);
            return frameInfo;
        }
    }
}
