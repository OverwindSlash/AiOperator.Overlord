using System;
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
            return CalculateDistance(toi) / toi.MotionInfo.PrevIntervalToiTimespan.TotalHours;
        }

        public double CalculateDistance(TrafficObjectInfo toi)
        {
            var llcs = MapToLonLatCoordinate(toi);
            double distanceTo = llcs.Item1.DistanceTo(llcs.Item2);

            return distanceTo;
        }

        private (LonLatCoordinate, LonLatCoordinate) MapToLonLatCoordinate(TrafficObjectInfo toi)
        {
            UvCoordinate uvcCurrent = new UvCoordinate(toi.CenterX, toi.CenterY);
            UvCoordinate uvcLast = new UvCoordinate(toi.CenterX - toi.MotionInfo.XOffset, toi.CenterY - toi.MotionInfo.YOffset);

            LonLatCoordinate llcCurrent = _mapper.Uv2LonLatCoordinate(uvcCurrent);
            LonLatCoordinate llcLast = _mapper.Uv2LonLatCoordinate(uvcLast);
            return (llcCurrent, llcLast);
        }

        public double CalculateDirection(TrafficObjectInfo toi)
        {
            var llcs = MapToLonLatCoordinate(toi);
            return DegreeBearing(llcs.Item1.Latitude, llcs.Item1.Longitude, llcs.Item2.Latitude, llcs.Item2.Longitude);
        }

        private double DegreeBearing(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = ToRad(lon2 - lon1);

            var dPhi = Math.Log(
                Math.Tan(ToRad(lat2) / 2 + Math.PI / 4) / Math.Tan(ToRad(lat1) / 2 + Math.PI / 4));

            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);

            return ToBearing(Math.Atan2(dLon, dPhi));
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }
    }
}
