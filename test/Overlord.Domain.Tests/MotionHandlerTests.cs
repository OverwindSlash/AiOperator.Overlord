using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class MotionHandlerTests
    {
        public void TestGenerateMotionHandler()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/pl_000001.jpg", ImreadModes.Color);

            FrameInfo frameInfo = new FrameInfo(1L, mat);
            string json = File.ReadAllText("Json/pl_000001.json");
            frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);

            MotionHandler handler = new MotionHandler();
            handler.SetRoadDefinition(roadDefinition);
            handler.Analyze(frameInfo);

            Assert.NotNull(frameInfo);
        }

        [Test]
        public void TestMotionHandler_OneFrame()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);

            using Mat mat = new Mat("Images/pl_000001.jpg", ImreadModes.Color);

            FrameInfo frameInfo = new FrameInfo(1L, mat);
            string json = File.ReadAllText("Json/pl_000001.json");
            frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);

            MotionHandler handler = new MotionHandler();
            handler.SetRoadDefinition(roadDefinition);
            handler.Analyze(frameInfo);

            Assert.AreEqual(1, handler.Service.GetMotionHistoryById(frameInfo.ObjectInfos[0].Id).Count);
        }

        [Test]
        public void TestMotionHandler_TwoFrames()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MotionCalculationFrameInterval = 1;
            roadDefinition.SetImageSize(1920, 1080);

            // frame1
            using Mat mat1 = new Mat("Images/pl_000001.jpg", ImreadModes.Color);
            FrameInfo frameInfo1 = new FrameInfo(1L, mat1);
            string json1 = File.ReadAllText("Json/pl_000001.json");
            frameInfo1.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json1);

            // frame2
            using Mat mat2 = new Mat("Images/pl_000002.jpg", ImreadModes.Color);
            FrameInfo frameInfo2 = new FrameInfo(2L, mat2);
            string json2 = File.ReadAllText("Json/pl_000002.json");
            frameInfo2.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json2);

            MotionHandler handler = new MotionHandler();
            handler.SetRoadDefinition(roadDefinition);

            handler.Analyze(frameInfo1);
            handler.Analyze(frameInfo2);

            SortedList<long, TrafficObjectInfo> motionHistoryById = handler.Service.GetMotionHistoryById("truck:1");
            Assert.AreEqual(2, motionHistoryById.Count);

            TrafficObjectInfo toi1 = motionHistoryById[1];
            Assert.AreEqual(1078, toi1.CenterX);
            Assert.AreEqual(817, toi1.CenterY);

            TrafficObjectInfo toi2 = motionHistoryById[2];
            Assert.AreEqual(1, toi2.MotionInfo.LastToiFrameId);
            Assert.AreEqual(200, toi2.MotionInfo.LastToiTimespan.TotalMilliseconds);
            Assert.AreEqual(1124, toi2.CenterX);
            Assert.AreEqual(765, toi2.CenterY);
            Assert.AreEqual(46, toi2.MotionInfo.XOffset);
            Assert.AreEqual(-52, toi2.MotionInfo.YOffset);
            Assert.AreEqual(69.426, toi2.MotionInfo.Offset, 0.001);
        }

        [Test]
        public void TestMotionHandler_TenFrames()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.MotionCalculationFrameInterval = 5;
            roadDefinition.SetImageSize(1920, 1080);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);

            MotionHandler handler = new MotionHandler();
            handler.SetRoadDefinition(roadDefinition);

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

                handler.Analyze(frameInfo);
            }

            SortedList<long, TrafficObjectInfo> motionHistoryById = handler.Service.GetMotionHistoryById("truck:1");
            Assert.AreEqual(10, motionHistoryById.Count);

            TrafficObjectInfo toi1 = motionHistoryById[1];
            TrafficObjectInfo toi2 = motionHistoryById[2];
            TrafficObjectInfo toi5 = motionHistoryById[5];
            TrafficObjectInfo toi6 = motionHistoryById[6];
            TrafficObjectInfo toi7 = motionHistoryById[7];
            TrafficObjectInfo toi10 = motionHistoryById[10];

            // boundary of frame not be calculated 
            Assert.AreEqual(0, toi5.MotionInfo.XOffset);
            Assert.AreEqual(0, toi5.MotionInfo.YOffset);
            Assert.AreEqual(0, toi5.MotionInfo.LastToiFrameId);
            Assert.AreEqual(0, toi5.MotionInfo.LastToiTimespan.TotalMilliseconds);

            // first calculated frame
            Assert.AreEqual(toi6.CenterX - toi1.CenterX, toi6.MotionInfo.XOffset);
            Assert.AreEqual(toi6.CenterY - toi1.CenterY, toi6.MotionInfo.YOffset);
            Assert.AreEqual(1, toi6.MotionInfo.LastToiFrameId);
            Assert.AreEqual(1000, toi6.MotionInfo.LastToiTimespan.TotalMilliseconds);

            // second frame
            Assert.AreEqual(toi7.CenterX - toi2.CenterX, toi7.MotionInfo.XOffset);
            Assert.AreEqual(toi7.CenterY - toi2.CenterY, toi7.MotionInfo.YOffset);
            Assert.AreEqual(2, toi7.MotionInfo.LastToiFrameId);
            Assert.AreEqual(1000, toi7.MotionInfo.LastToiTimespan.TotalMilliseconds);

            // last frame
            Assert.AreEqual(toi10.CenterX - toi5.CenterX, toi10.MotionInfo.XOffset);
            Assert.AreEqual(toi10.CenterY - toi5.CenterY, toi10.MotionInfo.YOffset);
            Assert.AreEqual(5, toi10.MotionInfo.LastToiFrameId);
            Assert.AreEqual(1000, toi10.MotionInfo.LastToiTimespan.TotalMilliseconds);
        }
    }
}
