using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Services;
using System.Collections.Concurrent;

namespace Overlord.Domain.Handlers
{
    public class CountingHandler : AnalysisHandlerBase
    {
        private CountingService _countingService;

        public ConcurrentDictionary<string, int> ObjsCrossedEnterLine => _countingService.ObjsCrossedEnterLine;
        public ConcurrentDictionary<string, int> ObjsCrossedLeaveLine => _countingService.ObjsCrossedLeaveLine;
        public ConcurrentDictionary<string, long> ObjsCounted => _countingService.ObjsCounted;

        public CountingHandler()
        {
            _countingService = new CountingService();
        }

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _countingService.SetRoadDefinition(roadDefinition);
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
            {
                _countingService.CountTrafficeObject(toi);
            }
            
            return frameInfo;
        }

        public int GetLaneObjectTypeCount(int laneId, string typeName)
        {
            return _countingService.GetLaneObjectTypeCount(laneId, typeName);
        }
    }
}
