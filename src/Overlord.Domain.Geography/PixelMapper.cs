using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NumSharp;
using NumSharp.Backends.Unmanaged;
using OpenCvSharp;

namespace Overlord.Domain.Geography
{
    public class PixelMapper
    {
        private static Mat _transform;
        private static Mat _revTransform;

        public static void SetUvAndLonLatQuadrilateral(UvQuadrilateral uvQuad, LonLatQuadrilateral lonLatQuad)
        {
            _transform = Cv2.GetPerspectiveTransform(uvQuad.ToInputArray(), lonLatQuad.ToInputArray());
            _revTransform = Cv2.GetPerspectiveTransform(lonLatQuad.ToInputArray(), uvQuad.ToInputArray());
        }

        public static (double, double) Uv2LonLatCoordinate(UvCoordinate uvCoordinate)
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
    }
}
