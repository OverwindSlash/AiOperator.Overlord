using NumSharp;
using Overlord.Core.Entities.Geometric;

namespace Overlord.Domain.Geography
{
    public class UvCoordinate
    {
        public int X { get; }

        public int Y { get; }

        public UvCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public UvCoordinate(NormalizedPoint point)
        {
            X = point.OriginalX;
            Y = point.OriginalY;
        }

        public NDArray ToArray()
        {
            NDArray array = new NDArray(typeof(int), 2);
            array[0] = X;
            array[1] = Y;
            return array;
        }
    }
}
