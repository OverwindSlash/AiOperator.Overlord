using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using System.Collections.Generic;
using System.Threading;
using Overlord.Domain.Event;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class RegionHandlerTests
    {
        [Test]
        public void TestGenerateNewRegionHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/demoRd.json");
            roadDefinition.SetImageSize(1920, 1080);

            RegionHandler handler = new RegionHandler();
            handler.SetRoadDefinition(roadDefinition);

            FrameInfo frameInfo = new FrameInfo(0L, new Mat());
            frameInfo.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo(){X = 1200, Y = 800});
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo(){X = 500, Y = 1000});
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo(){X = 1050, Y = 950});

            handler.Analyze(frameInfo);

            Assert.True(frameInfo.TrafficObjectInfos[0].IsAnalyzable);
            Assert.True(frameInfo.TrafficObjectInfos[1].IsAnalyzable);
            Assert.False(frameInfo.TrafficObjectInfos[2].IsAnalyzable);
        }

        [Test]
        public void TestIncludeAndExcludeRegionHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/cj_k32_500.json");
            roadDefinition.SetImageSize(1280, 720);

            RegionHandler handler = new RegionHandler();
            handler.SetRoadDefinition(roadDefinition);

            FrameInfo frameInfo = new FrameInfo(0L, new Mat());
            frameInfo.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo() { X = 700, Y = 600 });
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo() { X = 1200, Y = 600 });
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo() { X = 300, Y = 300 });
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo() { X = 800, Y = 350 });
            frameInfo.TrafficObjectInfos.Add(new TrafficObjectInfo() { X = 1100, Y = 440 });

            handler.Analyze(frameInfo);

            Assert.True(frameInfo.TrafficObjectInfos[0].IsAnalyzable);
            Assert.False(frameInfo.TrafficObjectInfos[1].IsAnalyzable);
            Assert.False(frameInfo.TrafficObjectInfos[2].IsAnalyzable);
            Assert.True(frameInfo.TrafficObjectInfos[3].IsAnalyzable);
            Assert.False(frameInfo.TrafficObjectInfos[4].IsAnalyzable);
        }

        [Test]
        public void TestObjectAnalyzableRetainRegionHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/cj_k32_500.json");
            roadDefinition.IsObjectAnalyzableRetain = true;
            roadDefinition.SetImageSize(1280, 720);

            RegionHandler handler = new RegionHandler();
            handler.SetRoadDefinition(roadDefinition);

            FrameInfo frameInfo1 = new FrameInfo(1L, new Mat());
            frameInfo1.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo1.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1, X = 700, Y = 600 });
            handler.Analyze(frameInfo1);

            FrameInfo frameInfo2 = new FrameInfo(2L, new Mat());
            frameInfo2.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo2.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1, X = 1200, Y = 600 });
            frameInfo2.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 2, X = 300, Y = 300 });
            frameInfo2.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 3, X = 1100, Y = 440 });
            handler.Analyze(frameInfo2);

            FrameInfo frameInfo3 = new FrameInfo(3L, new Mat());
            frameInfo3.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo3.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 3, X = 1100, Y = 400 });
            handler.Analyze(frameInfo3);

            Assert.True(frameInfo1.TrafficObjectInfos[0].IsAnalyzable);
            Assert.True(frameInfo2.TrafficObjectInfos[0].IsAnalyzable);
            Assert.False(frameInfo2.TrafficObjectInfos[1].IsAnalyzable);
            Assert.False(frameInfo2.TrafficObjectInfos[2].IsAnalyzable);
            Assert.True(frameInfo3.TrafficObjectInfos[0].IsAnalyzable);
        }

        [Test]
        public void TestObjectExpiredRetainRegionHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/cj_k32_500.json");
            roadDefinition.IsObjectAnalyzableRetain = true;
            roadDefinition.SetImageSize(1280, 720);

            RegionHandler handler = new RegionHandler();
            handler.SetRoadDefinition(roadDefinition);

            FrameInfo frameInfo1 = new FrameInfo(1L, new Mat());
            frameInfo1.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo1.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1, X = 700, Y = 600 });
            handler.Analyze(frameInfo1);

            FrameInfo frameInfo2 = new FrameInfo(2L, new Mat());
            frameInfo2.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo2.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1, X = 1200, Y = 600 });
            frameInfo2.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 2, X = 300, Y = 300 });
            frameInfo2.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 3, X = 1100, Y = 440 });
            handler.Analyze(frameInfo2);

            FrameInfo frameInfo3 = new FrameInfo(3L, new Mat());
            frameInfo3.TrafficObjectInfos = new List<TrafficObjectInfo>();
            frameInfo3.TrafficObjectInfos.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 3, X = 1100, Y = 400 });
            handler.Analyze(frameInfo3);

            Assert.AreEqual(2, handler.Service.GetAnalyzableObjectCount());
            handler.Service.OnNext(new ObjectExpiredEvent("car:1", 1, "car", 1));
            Thread.Sleep(1000);
            Assert.AreEqual(1, handler.Service.GetAnalyzableObjectCount());
        }
    }
}
