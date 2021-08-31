using System.Collections.Generic;
using System.Drawing;

namespace Overlord.Domain.ObjectTracker.Tests
{
    public class Frame
    {
        public Frame(List<RectangleF> boundingBoxes)
        {
            BoundingBoxes = boundingBoxes;
        }
        public List<RectangleF> BoundingBoxes { get; set; }
    }
}
