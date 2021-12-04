using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Geometric;
using Overlord.Core.Entities.Road;
using System.Collections.Generic;

namespace Overlord.Domain.Services
{
    public class LaneService
    {
        private RoadDefinition _roadDefinition;
        private List<Lane> _lanes;

        public const int NotNeedToCalculateLane = -1;
        public const int WrongLaneIndex = 0;

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _roadDefinition = roadDefinition;
            _lanes = _roadDefinition.Lanes;
        }

        public int CalculateLane(TrafficObjectInfo toi)
        {
            toi.WasLaneCalculated = true;

            if (!toi.IsAnalyzable)
            {
                return NotNeedToCalculateLane;
            }

            NormalizedPoint point = new NormalizedPoint(
                _roadDefinition.ImageWidth, _roadDefinition.ImageHeight, toi.BottomCenterX, toi.BottomCenterY);

            foreach (Lane lane in _lanes)
            {
                if (lane.IsPointInPolygon(point))
                {
                    return lane.Index;
                }
            }

            return WrongLaneIndex;
        }
    }
}
