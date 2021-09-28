using OpenCvSharp;
using System;

namespace Overlord.Core.Entities.Frame
{
    public class TrafficObjectInfo : IDisposable
    {
        public long FrameId { get; set; }
        public DateTime TimeStamp { get; set; }

        // traffic object type and id
        public string Id => $"{Type}:{TrackingId}";
        public int TypeId { get; set; }
        public string Type { get; set; }
        public long TrackingId { get; set; }
        public float Confidence { get; set; }

        // BBox
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CenterX => (X + Width / 2);
        public int CenterY => (Y + Height / 2);
        public int BottomCenterX => (X + Width / 2);
        public int BottomCenterY => (Y + Height);
        public int BottomLeftX => X;
        public int BottomLeftY => (Y + Height);
        public int BottomRightX => (X + Width);
        public int BottomRightY => BottomLeftY;

        // processed flag of each analysis step
        public bool IsAnalyzable { get; set; }
        public bool IsCaptured { get; set; }
        public bool IsLaneCalculated { get; set; }
        public bool IsPlateRecognized { get; set; }
        public bool IsMotionCalculated => MotionInfo.IsMotionCalculated;
        public bool IsCounted { get; set; }
        public bool IsEventDetected { get; set; }
        
        // processed result of each analysis step
        public Mat Snapshot { get; set; }
        public int LaneIndex { get; set; }
        public PlateInfo PlateInfo { get; set; }
        public MotionInfo MotionInfo { get; set; }

        // Status occurred flag
        public bool InStatusSlowSpeed { get; set; }
        public bool InStatusFastSpeed { get; set; }
        public bool InStatusCrushLine { get; set; }
        public bool InStatusLaneChanged { get; set; }
        public bool InStatusEnterForbiddenRegion { get; set; }
        public bool InStatusStopped { get; set; }
        public bool InStatusReversed { get; set; }
        public bool InStatusWasteDropped { get; set; }
        public bool InStatusUnderConstruction { get; set; }
        public bool InStatusDangerousGoods { get; set; }

        // Event raised
        public bool EventStoppedVehicleRaised { get; set; }
        public bool EventRoadJamRaised { get; set; }
        public bool EventSlowVehicleRaised { get; set; }
        public bool EventRoadAmbleRaised { get; set; }
        public bool EventForbiddenTypeRaised { get; set; }
        public bool InEventPersonTrespass { get; set; }
        public bool InEventTruckTrespass { get; set; }
        public bool InEventNonMotorTrespass { get; set; }
        public bool InEventEmergencyLaneOccupied { get; set; }
        

        public TrafficObjectInfo()
        {
            PlateInfo = new PlateInfo();
            MotionInfo = new MotionInfo();
        }

        public bool IsContainObject(TrafficObjectInfo toi)
        {
            return (X < toi.X) && (Y < toi.Y) && ((X + Width) > (toi.X + toi.Width)) &&
                   ((Y + Height) > (toi.Y + toi.Height));
        }

        public void Dispose()
        {
            Snapshot?.Dispose();
        }
    }
}