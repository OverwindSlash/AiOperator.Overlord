using NUnit.Framework;
using Overlord.Core.Entities.Road;

namespace Overlord.Domain.Geography.Tests
{
    [TestFixture]
    public class PixelMapperTests
    {
        private PixelMapper _mapper;

        [SetUp]
        public void Initialize()
        {
            // List<int> uvcs = new List<int>();
            // uvcs.AddRange(new[] { 1326, 598, 901, 620, 1025, 299, 1131, 276 });
            //
            // List<float> llcs = new List<float>();
            // llcs.AddRange(new[]
            // {
            //     (float)120.6586438007756, (float)31.224101211225364, (float)120.65872464827149, (float)31.22401242993521,
            //     (float)120.65726041029022, (float)31.223541501683588, (float)120.65684269822808, (float)31.223684324436487
            // });

            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);

            _mapper = new PixelMapper(roadDefinition.UvQuadrilateral, roadDefinition.LonLatQuadrilateral);
        }

        [Test]
        public void TestCreatePixelMapper()
        {
            UvCoordinate uvc1 = new UvCoordinate(1326, 598);
            var lonlat = _mapper.Uv2LonLatDouble(uvc1);

            Assert.AreEqual(120.6586438007756, lonlat.Item1, 0.00001);
            Assert.AreEqual(31.224101211225364, lonlat.Item2, 0.00001);
        }

        [Test]
        public void TestCalculateDistance()
        {
            UvCoordinate uvcStart = new UvCoordinate(1326, 598);
            UvCoordinate uvcStop = new UvCoordinate(1131, 276);
            var lonlatStart = _mapper.Uv2LonLatDouble(uvcStart);
            var lonlatStop = _mapper.Uv2LonLatDouble(uvcStop);

            LonLatCoordinate llcStart = new LonLatCoordinate((float)lonlatStart.Item1, (float)lonlatStart.Item2);
            LonLatCoordinate llcStop = new LonLatCoordinate((float)lonlatStop.Item1, (float)lonlatStop.Item2);

            double distanceTo = llcStart.DistanceTo(llcStop);
            Assert.AreEqual(0.177, distanceTo, 0.001);
        }

        [Test]
        public void TestPixelMapperGenerateTrajectory()
        {
            UvCoordinate path1 = new UvCoordinate(1078, 817);
            UvCoordinate path2 = new UvCoordinate(1124, 765);
            UvCoordinate path3 = new UvCoordinate(1151, 717);
            UvCoordinate path4 = new UvCoordinate(1174, 675);
            UvCoordinate path5 = new UvCoordinate(1193, 630);
            UvCoordinate path6 = new UvCoordinate(1204, 600);
            UvCoordinate path7 = new UvCoordinate(1220, 572);
            UvCoordinate path8 = new UvCoordinate(1225, 540);
            UvCoordinate path9 = new UvCoordinate(1235, 517);
            UvCoordinate path10 = new UvCoordinate(1247, 499);

            var lonlat1 = _mapper.Uv2LonLatDouble(path1);
            var lonlat2 = _mapper.Uv2LonLatDouble(path2);
            var lonlat3 = _mapper.Uv2LonLatDouble(path3);
            var lonlat4 = _mapper.Uv2LonLatDouble(path4);
            var lonlat5 = _mapper.Uv2LonLatDouble(path5);
            var lonlat6 = _mapper.Uv2LonLatDouble(path6);
            var lonlat7 = _mapper.Uv2LonLatDouble(path7);
            var lonlat8 = _mapper.Uv2LonLatDouble(path8);
            var lonlat9 = _mapper.Uv2LonLatDouble(path9);
            var lonlat10 = _mapper.Uv2LonLatDouble(path10);
        }
    }
}