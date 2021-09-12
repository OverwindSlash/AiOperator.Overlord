using System;

namespace Overlord.Core.Entities.Frame
{
    public class MotionInfo
    {
        public long LastToiFrameId { get; set; }
        public TimeSpan LastToiTimespan { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public double Offset { get; set; }

        public bool IsMotionCalculated { get; set; }
        public double Speed { get; set; }
        public double Direction { get; set; }
        public double Distance { get; set; }
    }
}