using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using System.Collections.Generic;

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
            frameInfo.ObjectInfos = new List<TrafficObjectInfo>();
            frameInfo.ObjectInfos.Add(new TrafficObjectInfo(){X = 1200, Y = 800});
            frameInfo.ObjectInfos.Add(new TrafficObjectInfo(){X = 500, Y = 1000});
            frameInfo.ObjectInfos.Add(new TrafficObjectInfo(){X = 1050, Y = 950});

            handler.Analyze(frameInfo);

            Assert.True(frameInfo.ObjectInfos[0].IsAnalyzable);
            Assert.True(frameInfo.ObjectInfos[1].IsAnalyzable);
            Assert.False(frameInfo.ObjectInfos[2].IsAnalyzable);
        }
    }
}
