using NSubstitute;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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

            FrameInfo frameInfo = new FrameInfo(1L, mat);
            handler.Analyze(frameInfo);
            
            Assert.AreEqual(14, frameInfo.ObjectInfos.Count);
            Assert.AreEqual(1L, frameInfo.ObjectInfos[0].FrameId);
            Assert.AreEqual(frameInfo.TimeStamp, frameInfo.ObjectInfos[0].TimeStamp);
        }

        [Test]
        public void TestCreateNewDetectionHandler_RealRoadDefAndImage()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/Traffic_002.jpg", ImreadModes.Color);

            // mock for detector
            IObjectDetector detector = Substitute.For<IObjectDetector>();
            string json = File.ReadAllText("Json/Traffic002_AnalysisResult.json");
            List<TrafficObjectInfo> trafficObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
            detector.Detect(mat, 0.6f).Returns(trafficObjectInfos);

            DetectionHandler handler = new DetectionHandler(detector);
            handler.SetRoadDefinition(roadDefinition);

            FrameInfo frameInfo = new FrameInfo(0L, mat);
            handler.Analyze(frameInfo);

            Assert.AreEqual(10, frameInfo.ObjectInfos.Count);
        }
    }
}
