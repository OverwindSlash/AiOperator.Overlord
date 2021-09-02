using NUnit.Framework;

namespace Overlord.Domain.Geography.Tests
{
    [TestFixture]
    public class PixelMapperTests
    {
        [Test]
        public void TestCreatePixelMapper()
        {
            UvCoordinate uvc1 = new UvCoordinate(1326, 598);
            UvCoordinate uvc2 = new UvCoordinate(901, 620);
            UvCoordinate uvc3 = new UvCoordinate(1025, 299);
            UvCoordinate uvc4 = new UvCoordinate(1131, 276);
            UvQuadrilateral uvq = new UvQuadrilateral(uvc1, uvc2, uvc3, uvc4);

            LonLatCoordinate llc1 = new LonLatCoordinate((float)120.6586438007756, (float)31.224101211225364);
            LonLatCoordinate llc2 = new LonLatCoordinate((float)120.65872464827149, (float)31.22401242993521);
            LonLatCoordinate llc3 = new LonLatCoordinate((float)120.65726041029022, (float)31.223541501683588);
            LonLatCoordinate llc4 = new LonLatCoordinate((float)120.65684269822808, (float)31.223684324436487);
            LonLatQuadrilateral llq = new LonLatQuadrilateral(llc1, llc2, llc3, llc4);

            PixelMapper.SetUvAndLonLatQuadrilateral(uvq, llq);
            var lonlat = PixelMapper.Uv2LonLatCoordinate(uvc1);

            Assert.AreEqual(120.6586438007756, lonlat.Item1, 0.00001);
            Assert.AreEqual(31.224101211225364, lonlat.Item2, 0.00001);

            // Calculate distance
            UvCoordinate uvcStart = new UvCoordinate(1326, 598);
            UvCoordinate uvcStop = new UvCoordinate(1131, 276);
            var lonlatStart = PixelMapper.Uv2LonLatCoordinate(uvcStart);
            var lonlatStop = PixelMapper.Uv2LonLatCoordinate(uvcStop);

            LonLatCoordinate llcStart = new LonLatCoordinate((float)lonlatStart.Item1, (float)lonlatStart.Item2);
            LonLatCoordinate llcStop = new LonLatCoordinate((float)lonlatStop.Item1, (float)lonlatStop.Item2);

            double distanceTo = llcStart.DistanceTo(llcStop);
            Assert.AreEqual(0.177, distanceTo, 0.001);
        }
    }
}