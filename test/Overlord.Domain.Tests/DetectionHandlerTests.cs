using NSubstitute;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using System.Collections.Generic;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class DetectionHandlerTests
    {
        [Test]
        public void TestCreateNewDetectionHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/demoRd.json");
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/Traffic_001.jpg", ImreadModes.Color);

            // mock for detector
            IObjectDetector detector = Substitute.For<IObjectDetector>();
            List<TrafficObjectInfo> trafficObjectInfos = new List<TrafficObjectInfo>();
            for (int i = 0; i < 14; i++)
            {
                trafficObjectInfos.Add(new TrafficObjectInfo());
            }
            detector.Detect(mat, 0.7f).Returns(trafficObjectInfos);

            DetectionHandler handler = new DetectionHandler(detector);
            handler.SetRoadDefinition(roadDefinition);

            FrameInfo frameInfo = new FrameInfo(0L, mat);
            handler.Analyze(frameInfo);
            
            Assert.AreEqual(14, frameInfo.ObjectInfos.Count);
        }
    }
}
