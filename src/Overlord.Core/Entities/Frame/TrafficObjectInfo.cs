using OpenCvSharp;
using System;

namespace Overlord.Core.Entities.Frame
{
    public class TrafficObjectInfo : IDisposable
    {
        public long FrameId { get; set; }
        public DateTime TimeStamp { get; set; }

        // object type and id
        public string Id => $"{Type}:{TrackingId}";
        public int TypeId { get; set; }
        public string Type { get; set; }
        public float Confidence { get; set; }
        public long TrackingId { get; set; }

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
        public bool WasSnapshoted { get; set; }
        public bool WasLaneCalculated { get; set; }
        public bool WasPlateRecognized => PlateInfo.IsPlateRecognized;
        public bool WasSpeedCalculated => MotionInfo.IsSpeedCalculated;
        public bool WasCounted { get; set; }
        public bool WasEventDetected { get; set; }
        
        // processed result of each analysis step
        public Mat Snapshot { get; set; }
        public int LaneIndex { get; set; }
        public PlateInfo PlateInfo { get; set; }
        public MotionInfo MotionInfo { get; set; }

        // Status occurred flag
        // velocity related
        public bool InStatusSlowSpeed { get; set; }
        public bool InStatusFastSpeed { get; set; }
        public bool InStatusStopped { get; set; }
        public bool InStatusReversed { get; set; }

        // region related
        public bool InStatusCrushLine { get; set; }
        public bool InStatusLaneChanged { get; set; }
        public bool InStatusEnterForbiddenRegion { get; set; }
        
        // detection related
        public bool InStatusWasteDropped { get; set; }
        public bool InStatusUnderConstruction { get; set; }
        public bool InStatusDangerousGoods { get; set; }

        // TODO: Consider removing event raised flags to FrameInfo
        // Event raised
        // velocity related
        public bool EventSlowVehicleRaised { get; set; }
        public bool EventFastVehicleRaised { get; set; }
        public bool EventStoppedVehicleRaised { get; set; }
        public bool EventReversedVehicleRaised { get; set; }
        public bool EventRoadAmbleRaised { get; set; }
        public bool EventRoadJamRaised { get; set; }

        // region related
        public bool EventCrushLineRaised { get; set; }
        public bool EventLaneChangedRaised { get; set; }
        public bool EventForbiddenTypeRaised { get; set; }
        public bool EventPersonTrespassRaised { get; set; }
        public bool EventTruckTrespassRaised { get; set; }
        public bool EventNonMotorTrespassRaised { get; set; }
        public bool EventEmergencyLaneOccupiedRaised { get; set; }
        
        // detection related
        public bool EventWasteDroppedRaised { get; set; }
        public bool EventUnderConstructionRaised { get; set; }
        public bool EventDangerousGoodsRaised { get; set; }
        
        // complex event
        public bool EventTrafficAccidentRaised { get; set; }
        public bool EventRoadRescueRaised { get; set; }
        public bool EventQueueOverrunRaised { get; set; }
        public bool EventIllegalDropOffRaised { get; set; }
        public bool EventCameraMovementRaised { get; set; }

        public TrafficObjectInfo()
        {
            PlateInfo = new PlateInfo();
            MotionInfo = new MotionInfo();
        }

        // check whether a object is within this object
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