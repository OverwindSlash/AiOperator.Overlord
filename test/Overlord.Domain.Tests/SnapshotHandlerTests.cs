using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using Overlord.Domain.Services;

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
            Assert.AreEqual(1, service.GetCacheSceneCount());
        }
    }
}
