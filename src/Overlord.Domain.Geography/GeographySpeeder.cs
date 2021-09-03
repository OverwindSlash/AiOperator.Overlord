using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Interfaces;

namespace Overlord.Domain.Geography
{
    public class GeographySpeeder : ISpeeder
    {
        private PixelMapper _mapper;

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _mapper = new PixelMapper(roadDefinition.UvQuadrilateral, roadDefinition.LonLatQuadrilateral);
        }

        public double CalculateSpeed(TrafficObjectInfo toi)
        {
            UvCoordinate uvcStart = new UvCoordinate(toi.CenterX, toi.CenterY);
            UvCoordinate uvcStop = new UvCoordinate(toi.CenterX - toi.MotionInfo.XOffset, toi.CenterY - toi.MotionInfo.YOffset);

            LonLatCoordinate llcStart = _mapper.Uv2LonLatCoordinate(uvcStart);
            LonLatCoordinate llcStop = _mapper.Uv2LonLatCoordinate(uvcStop);
            double distanceTo = llcStart.DistanceTo(llcStop);

            return distanceTo / toi.MotionInfo.LastToiTimespan.TotalHours;
        }
    }
}
