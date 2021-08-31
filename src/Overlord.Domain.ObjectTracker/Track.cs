using System.Collections.Generic;
using System.Drawing;

namespace Overlord.Domain.ObjectTracker
{
    public record Track
    {
        public int TrackId { get; set; }

        public int TotalMisses { get; set; }

        public int Misses { get; set; }

        public List<RectangleF> History { get; set; }

        public TrackState State { get; set; }

        public RectangleF Prediction { get; set; }
    }
}