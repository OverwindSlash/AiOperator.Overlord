using NUnit.Framework;
using Overlord.Core.Entities.Road;
using System;
using System.Collections.Generic;

namespace Overlord.Domain.Geography.Tests
{
    [TestFixture]
    public class PixelMapperTests
    {
        private PixelMapper _mapper;

        private const int UvX1 = 1327;
        private const int UvY1 = 599;

        private const int UvX2 = 1312;
        private const int UvY2 = 276;

        private const int UvX3 = 1026;
        private const int UvY3 = 299;

        private const int UvX4 = 900;
        private const int UvY4 = 621;

        private const double Lon1 = 120.6586424915274;
        private const double Lat1 = 31.22410363108796;

        private const double Lon2 = 120.65684588050745;
        private const double Lat2 = 31.223679024166824;

        private const double Lon3 = 120.65726808409714;
        private const double Lat3 = 31.22353620140584;

        private const double Lon4 = 120.6587233390233;
        private const double Lat4 = 31.22400712968413;

        // temp point
        private const int UvXDisPoint = 1325;
        private const int UvYDisPoint = 295;

        [SetUp]
        public void Initialize()
        {
            // List<int> uvcs = new List<int>();
            // uvcs.AddRange(new[] { UvX1, UvY1, UvX2, UvY2, UvX3, UvY3, UvX4, UvY4 });
            //
            // List<float> llcs = new List<float>();
            // llcs.AddRange(new[]
            // {
            //     (float)Lon1, (float)Lat1, (float)Lon2, (float)Lat2,
            //     (float)Lon3, (float)Lat3, (float)Lon4, (float)Lat4
            // });
            //
            // _mapper = new PixelMapper(uvcs, llcs);

            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);
            
            _mapper = new PixelMapper(roadDefinition.UvQuadrilateral, roadDefinition.LonLatQuadrilateral);
        }

        [Test]
        public void TestCreatePixelMapper()
        {
            UvCoordinate uvc1 = new UvCoordinate(UvX1, UvY1);
            var lonlat = _mapper.Uv2LonLatDouble(uvc1);

            Assert.AreEqual(Lon1, lonlat.Item1, 0.00001);
            Assert.AreEqual(Lat1, lonlat.Item2, 0.00001);
        }

        [Test]
        public void TestCalculateDistance()
        {
            UvCoordinate uvcStart = new UvCoordinate(UvX1, UvY1);
            UvCoordinate uvcStop = new UvCoordinate(UvX2, UvY2);
            var lonlatStart = _mapper.Uv2LonLatDouble(uvcStart);
            var lonlatStop = _mapper.Uv2LonLatDouble(uvcStop);

            LonLatCoordinate llcStart = new LonLatCoordinate((float)lonlatStart.Item1, (float)lonlatStart.Item2);
            LonLatCoordinate llcStop = new LonLatCoordinate((float)lonlatStop.Item1, (float)lonlatStop.Item2);

            double distanceTo = llcStart.DistanceTo(llcStop);
            Assert.AreEqual(0.177, distanceTo, 0.02);

            LonLatCoordinate llc1 = _mapper.Uv2LonLatCoordinate(new UvCoordinate(UvX1, UvY1));
            LonLatCoordinate llc2 = _mapper.Uv2LonLatCoordinate(new UvCoordinate(UvXDisPoint, UvYDisPoint));
            LonLatCoordinate llc3 = _mapper.Uv2LonLatCoordinate(new UvCoordinate(UvX2, UvY2));
            double distance1To2 = llc1.DistanceTo(llc2);
            double distance2To3 = llc2.DistanceTo(llc3);

            Assert.AreEqual(0.145, distance1To2, 0.002);
            Assert.AreEqual(0.033, distance2To3, 0.002);


            // convert to standard gps coordinates
            double[] gcj02Start = GpsUtil.Bd09ToGcj02(lonlatStart.Item1, lonlatStart.Item2);
            double[] gps84Start = GpsUtil.Gcj02ToGps84(gcj02Start[0], gcj02Start[1]);
            
            double[] gcj02Stop = GpsUtil.Bd09ToGcj02(lonlatStop.Item1, lonlatStop.Item2);
            double[] gps84Stop = GpsUtil.Gcj02ToGps84(gcj02Stop[0], gcj02Stop[1]);
            
            LonLatCoordinate gpsStart = new LonLatCoordinate((float)gps84Start[0], (float)gps84Start[1]);
            LonLatCoordinate gpsStop = new LonLatCoordinate((float)gps84Stop[0], (float)gps84Stop[1]);
            
            double distanceTo2 = gpsStart.DistanceTo(gpsStop);
            Assert.AreEqual(0.177, distanceTo, 0.002);
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

            LonLatCoordinate llc1 = _mapper.Uv2LonLatCoordinate(path1);
            LonLatCoordinate llc10 = _mapper.Uv2LonLatCoordinate(path10);

            double distanceTo = llc1.DistanceTo(llc10);
            double speed = distanceTo / (1.8 / 60.0 / 60.0);
        }

        [Test]
        public void TestPixelMapperConvertToOtherCoordinate()
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

            List<double> gcj02Gps = new List<double>();
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat1.Item1, lonlat1.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat2.Item1, lonlat2.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat3.Item1, lonlat3.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat4.Item1, lonlat4.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat5.Item1, lonlat5.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat6.Item1, lonlat6.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat7.Item1, lonlat7.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat8.Item1, lonlat8.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat9.Item1, lonlat9.Item2));
            gcj02Gps.AddRange(GpsUtil.Bd09ToGcj02(lonlat10.Item1, lonlat10.Item2));


            Console.Write("[");
            for (int i = 0; i < 20; i++)
            {
                Console.Write(gcj02Gps[i].ToString("F6"));

                if (i % 2 == 0)
                {
                    Console.Write(",");
                }
                else
                {
                    Console.WriteLine("],");
                    Console.Write("[");
                }
            }
        }
    }
}