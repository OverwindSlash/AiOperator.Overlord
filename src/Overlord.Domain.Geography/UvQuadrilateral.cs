using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace Overlord.Domain.Geography
{
    public class UvQuadrilateral
    {
        public UvCoordinate C1 { get; }
        public UvCoordinate C2 { get; }
        public UvCoordinate C3 { get; }
        public UvCoordinate C4 { get; }

        public UvQuadrilateral(UvCoordinate c1, UvCoordinate c2, UvCoordinate c3, UvCoordinate c4)
        {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
        }

        public InputArray ToInputArray()
        {
            Point2f[] point2Fs = new Point2f[4];
            point2Fs[0] = new Point2f(C1.X, C1.Y);
            point2Fs[1] = new Point2f(C2.X, C2.Y);
            point2Fs[2] = new Point2f(C3.X, C3.Y);
            point2Fs[3] = new Point2f(C4.X, C4.Y);

            InputArray array = InputArray.Create(point2Fs);
            return array;
        }
    }
}
