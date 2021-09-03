using System;
using System.Collections.Generic;
using NumSharp;
using OpenCvSharp;

namespace Overlord.Domain.Geography
{
    public class PixelMapper
    {
        private Mat _transform;

        public PixelMapper(List<int> uvQuad, List<float> lonlatQuad)
        {
            if (uvQuad.Count != 8)
            {
                throw new ArgumentException("pixel quadrilateral not correct.");
            }

            if (lonlatQuad.Count != 8)
            {
                throw new ArgumentException("longitude-latitude quadrilateral not correct.");
            }

            UvCoordinate uvc1 = new UvCoordinate(uvQuad[0], uvQuad[1]);
            UvCoordinate uvc2 = new UvCoordinate(uvQuad[2], uvQuad[3]);
            UvCoordinate uvc3 = new UvCoordinate(uvQuad[4], uvQuad[5]);
            UvCoordinate uvc4 = new UvCoordinate(uvQuad[6], uvQuad[7]);
            UvQuadrilateral uvq = new UvQuadrilateral(uvc1, uvc2, uvc3, uvc4);

            LonLatCoordinate llc1 = new LonLatCoordinate(lonlatQuad[0], lonlatQuad[1]);
            LonLatCoordinate llc2 = new LonLatCoordinate(lonlatQuad[2], lonlatQuad[3]);
            LonLatCoordinate llc3 = new LonLatCoordinate(lonlatQuad[4], lonlatQuad[5]);
            LonLatCoordinate llc4 = new LonLatCoordinate(lonlatQuad[6], lonlatQuad[7]);
            LonLatQuadrilateral llq = new LonLatQuadrilateral(llc1, llc2, llc3, llc4);

            SetUvAndLonLatQuadrilateral(uvq, llq);
        }

        public void SetUvAndLonLatQuadrilateral(UvQuadrilateral uvQuad, LonLatQuadrilateral lonLatQuad)
        {
            _transform = Cv2.GetPerspectiveTransform(uvQuad.ToInputArray(), lonLatQuad.ToInputArray());
        }

        public (double, double) Uv2LonLatDouble(UvCoordinate uvCoordinate)
        {
            _transform.GetRectangularArray<double>(out var data);

            var pixel = np.array(uvCoordinate.ToArray()).reshape(1, 2);
            pixel = np.concatenate(new NDArray[] { pixel, np.ones((pixel.shape[0], 1)) }, 1);

            NDArray lonlat = np.dot(data, pixel.T);
            double[] array = lonlat.ToArray<double>();

            double longitudeD = array[0] / array[2];
            double latitudeD = array[1] / array[2];

            return (longitudeD, latitudeD);
        }

        public LonLatCoordinate Uv2LonLatCoordinate(UvCoordinate uvCoordinate)
        {
            (double, double) lonLatDouble = Uv2LonLatDouble(uvCoordinate);

            return new LonLatCoordinate((float)lonLatDouble.Item1, (float)lonLatDouble.Item2);
        }
    }
}
