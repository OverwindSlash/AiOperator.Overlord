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
        public bool IsMotionCalculated { get; set; }
        public bool IsCounted { get; set; }
        public bool IsEventDetected { get; set; }
        
        // processed result of each analysis step
        public Mat Snapshot { get; set; }
        public int LaneIndex { get; set; }
        public PlateInfo PlateInfo { get; set; }
        public MotionInfo MotionInfo { get; set; }

        // Event occurred flag
        public bool InEventSlowSpeed { get; set; }
        public bool InEventFastSpeed { get; set; }
        public bool InEventCrushLine { get; set; }
        public bool InEventLaneChanged { get; set; }
        public bool InEventEnterForbiddenRegion { get; set; }
        public bool InEventPersonTrespass { get; set; }
        public bool InEventTruckTrespass { get; set; }
        public bool InEventNonMotorTrespass { get; set; }
        public bool InEventEmergencyLaneOccupied { get; set; }
        public bool InEventStopped { get; set; }
        public bool InEventReversed { get; set; }
        public bool InEventWaste{ get; set; }
        public bool InEventUnderConstruction{ get; set; }
        public bool InEventDangerousGoods{ get; set; }

        public void Dispose()
        {
            Snapshot?.Dispose();
        }
    }
}