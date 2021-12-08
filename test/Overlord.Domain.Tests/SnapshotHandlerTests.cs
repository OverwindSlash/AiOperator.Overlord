using System;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using Overlord.Domain.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class SnapshotHandlerTests
    {
        [Test]
        public void TestGenerateSnapshotHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/demoRd.json");
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/Traffic_001.jpg", ImreadModes.Color);

            FrameInfo frameInfo = new FrameInfo(1L, mat);
            string json = File.ReadAllText("Json/Traffic001_AnalysisResult.json");
            frameInfo.TrafficObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);

            SnapshotHandler handler = new SnapshotHandler();
            handler.SetRoadDefinition(roadDefinition);
            handler.Analyze(frameInfo);

            SnapshotService service = handler.Service;
            Assert.AreEqual(1, service.GetCachedSceneCount());
        }

        [Test]
        public void TestSnapshotHandler_NotExceedMaxObjectSnapshots()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.SetImageSize(1920, 1080);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            SnapshotHandler handler = new SnapshotHandler();
            handler.SetRoadDefinition(roadDefinition);

            for (int i = 1; i <= 10; i++)
            {
                string filename = $"Images/pl_0000{i:D2}.jpg";
                using Mat mat = new Mat(filename, ImreadModes.Color);

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/pl_0000{i:D2}.json");
                frameInfo.TrafficObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.TrafficObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                handler.Analyze(frameInfo);
            }

            Assert.AreEqual(10, handler.Service.GetCachedSceneCount());
            Assert.AreEqual(10, handler.Service.GetObjectSnapshotsByObjectId("truck:1").Count);
            Assert.AreEqual(10, handler.Service.GetObjectSnapshotsByObjectId("truck:2").Count);
            Assert.AreEqual(10, handler.Service.GetObjectSnapshotsByObjectId("car:1").Count);
            Assert.AreEqual(10, handler.Service.GetObjectSnapshotsByObjectId("car:2").Count);
            Assert.AreEqual(10, handler.Service.GetObjectSnapshotsByObjectId("person:1").Count);
            Assert.AreEqual(6, handler.Service.GetObjectSnapshotsByObjectId("truck:3").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("car:5").Count);
            Assert.AreEqual(4, handler.Service.GetObjectSnapshotsByObjectId("car:6").Count);
            Assert.AreEqual(3, handler.Service.GetObjectSnapshotsByObjectId("bus:1").Count);
            Assert.AreEqual(2, handler.Service.GetObjectSnapshotsByObjectId("car:15").Count);
            Assert.AreEqual(2, handler.Service.GetObjectSnapshotsByObjectId("car:4").Count);
            Assert.AreEqual(1, handler.Service.GetObjectSnapshotsByObjectId("car:12").Count);
            Assert.AreEqual(1, handler.Service.GetObjectSnapshotsByObjectId("car:13").Count);
            Assert.AreEqual(1, handler.Service.GetObjectSnapshotsByObjectId("car:14").Count);
        }


        [Test]
        public void TestSnapshotHandler_ExceedMaxObjectSnapshots()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MaxObjectSnapshots = 5;
            roadDefinition.SetImageSize(1920, 1080);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            SnapshotHandler handler = new SnapshotHandler();
            handler.SetRoadDefinition(roadDefinition);

            for (int i = 1; i <= 10; i++)
            {
                string filename = $"Images/pl_0000{i:D2}.jpg";
                using Mat mat = new Mat(filename, ImreadModes.Color);

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/pl_0000{i:D2}.json");
                frameInfo.TrafficObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.TrafficObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                handler.Analyze(frameInfo);
            }

            Assert.AreEqual(10, handler.Service.GetCachedSceneCount());
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("truck:1").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("truck:2").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("car:1").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("car:2").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("person:1").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("truck:3").Count);
            Assert.AreEqual(5, handler.Service.GetObjectSnapshotsByObjectId("car:5").Count);
            Assert.AreEqual(4, handler.Service.GetObjectSnapshotsByObjectId("car:6").Count);
            Assert.AreEqual(3, handler.Service.GetObjectSnapshotsByObjectId("bus:1").Count);
            Assert.AreEqual(2, handler.Service.GetObjectSnapshotsByObjectId("car:15").Count);
            Assert.AreEqual(2, handler.Service.GetObjectSnapshotsByObjectId("car:4").Count);
            Assert.AreEqual(1, handler.Service.GetObjectSnapshotsByObjectId("car:12").Count);
            Assert.AreEqual(1, handler.Service.GetObjectSnapshotsByObjectId("car:13").Count);
            Assert.AreEqual(1, handler.Service.GetObjectSnapshotsByObjectId("car:14").Count);
        }
    }
}
