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
    public class LaneHandlerTests
    {
        [Test]
        public void TestGenerateLaneHandler()
        {
            float detectThresh = 0.6f;

            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.DetectionThresh = detectThresh;
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/Traffic_002.jpg", ImreadModes.Color);

            // mock for detector
            IObjectDetector detector = Substitute.For<IObjectDetector>();
            string json = File.ReadAllText("Json/Traffic002_AnalysisResult.json");
            List<TrafficObjectInfo> trafficObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
            detector.Detect(mat, detectThresh).Returns(trafficObjectInfos);

            FrameInfo frameInfo = new FrameInfo(0L, mat);

            // 1. do detection
            DetectionHandler detectionHandler = new DetectionHandler(detector);
            detectionHandler.SetRoadDefinition(roadDefinition);
            detectionHandler.Analyze(frameInfo);

            // 2. do region check
            RegionHandler regionHandler = new RegionHandler();
            regionHandler.SetRoadDefinition(roadDefinition);
            regionHandler.Analyze(frameInfo);

            // 3. do lane calculation
            LaneHandler laneHandler = new LaneHandler();
            laneHandler.SetRoadDefinition(roadDefinition);
            laneHandler.Analyze(frameInfo);

            Assert.AreEqual(10, frameInfo.TrafficObjectInfos.Count);
            foreach (TrafficObjectInfo toi in frameInfo.TrafficObjectInfos)
            {
                Assert.True(toi.WasLaneCalculated);
            }
        }

        [Test]
        public void TestLaneHandlerAnalysis()
        {
            float detectThresh = 0.6f;

            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.DetectionThresh = detectThresh;
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/Traffic_002.jpg", ImreadModes.Color);

            // mock for detector
            IObjectDetector detector = Substitute.For<IObjectDetector>();
            string json = File.ReadAllText("Json/Traffic002_AnalysisResult.json");
            List<TrafficObjectInfo> trafficObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
            detector.Detect(mat, detectThresh).Returns(trafficObjectInfos);

            FrameInfo frameInfo = new FrameInfo(0L, mat);

            // 1. do detection
            DetectionHandler detectionHandler = new DetectionHandler(detector);
            detectionHandler.SetRoadDefinition(roadDefinition);
            detectionHandler.Analyze(frameInfo);

            // 2. do region check
            RegionHandler regionHandler = new RegionHandler();
            regionHandler.SetRoadDefinition(roadDefinition);
            regionHandler.Analyze(frameInfo);

            // 3. do lane calculation
            LaneHandler laneHandler = new LaneHandler();
            laneHandler.SetRoadDefinition(roadDefinition);
            laneHandler.Analyze(frameInfo);

            Assert.AreEqual(10, frameInfo.TrafficObjectInfos.Count);
            Assert.AreEqual(3, frameInfo.TrafficObjectInfos[0].LaneIndex);  // truck:1
            Assert.AreEqual(3, frameInfo.TrafficObjectInfos[1].LaneIndex);  // truck:2
            Assert.AreEqual(2, frameInfo.TrafficObjectInfos[2].LaneIndex);  // truck:3
            Assert.AreEqual(4, frameInfo.TrafficObjectInfos[3].LaneIndex);  // person:1
            Assert.AreEqual(-1, frameInfo.TrafficObjectInfos[4].LaneIndex); // car:1, out of analysis region
            Assert.AreEqual(2, frameInfo.TrafficObjectInfos[5].LaneIndex);  // car:2
            Assert.AreEqual(-1, frameInfo.TrafficObjectInfos[6].LaneIndex); // car:3, out of analysis region
            Assert.AreEqual(3, frameInfo.TrafficObjectInfos[7].LaneIndex);  // car:4
            Assert.AreEqual(-1, frameInfo.TrafficObjectInfos[8].LaneIndex); // car:5, out of analysis region
            Assert.AreEqual(4, frameInfo.TrafficObjectInfos[9].LaneIndex);  // car:6
        }
    }
}
