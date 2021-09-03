using OpenCvSharp;

namespace Overlord.Domain.Geography
{
    public class LonLatQuadrilateral
    {
        public LonLatCoordinate C1 { get; }
        public LonLatCoordinate C2 { get; }
        public LonLatCoordinate C3 { get; }
        public LonLatCoordinate C4 { get; }

        public LonLatQuadrilateral(LonLatCoordinate c1, LonLatCoordinate c2, LonLatCoordinate c3, LonLatCoordinate c4)
        {
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
        }

        public InputArray ToInputArray()
        {
            Point2f[] point2Fs = new Point2f[4];
            point2Fs[0] = new Point2f(C1.Longitude, C1.Latitude);
            point2Fs[1] = new Point2f(C2.Longitude, C2.Latitude);
            point2Fs[2] = new Point2f(C3.Longitude, C3.Latitude);
            point2Fs[3] = new Point2f(C4.Longitude, C4.Latitude);

            InputArray array = InputArray.Create(point2Fs);
            return array;
        }
    }
}
