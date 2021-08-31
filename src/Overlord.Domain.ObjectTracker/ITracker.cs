using System.Collections.Generic;
using System.Drawing;

namespace Overlord.Domain.ObjectTracker
{
    public interface ITracker
    {
        IEnumerable<Track> Track(IEnumerable<RectangleF> boxes);
    }
}