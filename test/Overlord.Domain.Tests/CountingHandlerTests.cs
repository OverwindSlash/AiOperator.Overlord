using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NSubstitute;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class CountingHandlerTests
    {
        [Test]
        public void TestGenerateCountingHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MotionCalculationFrameInterval = 5;
            roadDefinition.SetImageSize(1920, 1080);

            // mock speeder.
            ISpeeder speeder = Substitute.For<ISpeeder>();

            MotionHandler motionHandler = new MotionHandler(speeder);
            motionHandler.SetRoadDefinition(roadDefinition);

            CountingHandler countingHandler = new CountingHandler();
            countingHandler.SetRoadDefinition(roadDefinition);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            for (int i = 1; i <= 10; i++)
            {
                string filename = $"Images/pl_0000{i:D2}.jpg";
                using Mat mat = new Mat(filename, ImreadModes.Color);

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/pl_0000{i:D2}.json");
                frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                motionHandler.Analyze(frameInfo);
                countingHandler.Analyze(frameInfo);
            }

            Assert.AreEqual(3, countingHandler.ObjsCrossedEnterLine.Count);
            Assert.AreEqual(0, countingHandler.ObjsCrossedLeaveLine.Count);
        }

        [Test]
        public void TestCountingHandler_DoubleLaneCounting()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MotionCalculationFrameInterval = 5;
            roadDefinition.SetImageSize(1920, 1080);

            // mock speeder.
            ISpeeder speeder = Substitute.For<ISpeeder>();

            MotionHandler motionHandler = new MotionHandler(speeder);
            motionHandler.SetRoadDefinition(roadDefinition);

            CountingHandler countingHandler = new CountingHandler();
            countingHandler.SetRoadDefinition(roadDefinition);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            for (int i = 1; i <= 30; i++)
            {
                // use fake image.
                using Mat mat = new Mat();

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/count_0000{i:D2}.json");
                frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                motionHandler.Analyze(frameInfo);
                countingHandler.Analyze(frameInfo);
            }

            Assert.AreEqual(9, countingHandler.ObjsCrossedEnterLine.Count);
            Assert.AreEqual(5, countingHandler.ObjsCrossedLeaveLine.Count);
            Assert.AreEqual(4, countingHandler.ObjsCounted.Count);
        }

        [Test]
        public void TestCountingHandler_SingleLaneCounting()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MotionCalculationFrameInterval = 5;
            roadDefinition.IsDoubleLineCounting = false;
            roadDefinition.SetImageSize(1920, 1080);

            // mock speeder.
            ISpeeder speeder = Substitute.For<ISpeeder>();

            MotionHandler motionHandler = new MotionHandler(speeder);
            motionHandler.SetRoadDefinition(roadDefinition);

            CountingHandler countingHandler = new CountingHandler();
            countingHandler.SetRoadDefinition(roadDefinition);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            for (int i = 1; i <= 30; i++)
            {
                // use fake image.
                using Mat mat = new Mat();

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/count_0000{i:D2}.json");
                frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                motionHandler.Analyze(frameInfo);
                countingHandler.Analyze(frameInfo);
            }

            Assert.AreEqual(9, countingHandler.ObjsCrossedEnterLine.Count);
            Assert.AreEqual(9, countingHandler.ObjsCounted.Count);
        }

        [Test]
        public void TestCountingHandler_SingleLaneCountingWithLaneAnalysis()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MotionCalculationFrameInterval = 5;
            roadDefinition.IsDoubleLineCounting = false;
            roadDefinition.SetImageSize(1920, 1080);

            LaneHandler laneHandler = new LaneHandler();
            laneHandler.SetRoadDefinition(roadDefinition);

            // mock speeder.
            ISpeeder speeder = Substitute.For<ISpeeder>();

            MotionHandler motionHandler = new MotionHandler(speeder);
            motionHandler.SetRoadDefinition(roadDefinition);

            CountingHandler countingHandler = new CountingHandler();
            countingHandler.SetRoadDefinition(roadDefinition);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            for (int i = 1; i <= 30; i++)
            {
                // use fake image.
                using Mat mat = new Mat();

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/count_0000{i:D2}.json");
                frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                laneHandler.Analyze(frameInfo);
                motionHandler.Analyze(frameInfo);
                countingHandler.Analyze(frameInfo);
            }

            Assert.AreEqual(9, countingHandler.ObjsCrossedEnterLine.Count);
            Assert.AreEqual(9, countingHandler.ObjsCounted.Count);

            Assert.AreEqual(0, countingHandler.GetLaneObjectTypeCount(1, "car"));
            Assert.AreEqual(3, countingHandler.GetLaneObjectTypeCount(2, "car"));
            Assert.AreEqual(1, countingHandler.GetLaneObjectTypeCount(2, "truck"));
            Assert.AreEqual(2, countingHandler.GetLaneObjectTypeCount(3, "car"));
            Assert.AreEqual(2, countingHandler.GetLaneObjectTypeCount(3, "truck"));
            Assert.AreEqual(1, countingHandler.GetLaneObjectTypeCount(3, "bus"));
        }
    }
}
