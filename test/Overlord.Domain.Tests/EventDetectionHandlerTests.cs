using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using Overlord.Domain.EventAlg;
using Overlord.Domain.EventManagerSanbao;
using Overlord.Domain.Geography;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class EventDetectionHandlerTests
    {
        private string _reportApiUri = "http://localhost:21021/api/services/app/Demo/ReportEvent";
        
        [Test]
        public void TestGenerateForbiddenEventDetector()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);
            roadDefinition.DriveLaneForbiddenDurationFrame = 4;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 4;

            RegionHandler regionHandler = new RegionHandler();
            regionHandler.SetRoadDefinition(roadDefinition);

            SnapshotHandler snapshotHandler = new SnapshotHandler();
            snapshotHandler.SetRoadDefinition(roadDefinition);

            LaneHandler laneHandler = new LaneHandler();
            laneHandler.SetRoadDefinition(roadDefinition);

            EventDetectionHandler eventDetectionHandler = new EventDetectionHandler(snapshotHandler.Service);
            eventDetectionHandler.SetRoadDefinition(roadDefinition);

            ITrafficEventGenerator generator = new SanbaoEventGenerator();
            EventProcessor eventProcessor = new EventProcessor(30, generator);

            ITrafficEventPublisher publisher = new SanboEventPublisher(_reportApiUri);
            EventPublisher eventPublisher = new EventPublisher(publisher);

            string captureRoot = @"D:\Capture";
            ForbiddenTypeEventAlg forbiddenAlg = new ForbiddenTypeEventAlg(captureRoot, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(forbiddenAlg);

            ISpeeder speeder = new GeographySpeeder();
            MotionHandler motionHandler = new MotionHandler(speeder);
            motionHandler.SetRoadDefinition(roadDefinition);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);
            for (int i = 1; i <= 10; i++)
            {
                string filename = $"Images/pl_0000{i:D2}.jpg";
                Mat mat = new Mat(filename, ImreadModes.Color);

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/pl_0000{i:D2}.json");
                frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                regionHandler.Analyze(frameInfo);
                laneHandler.Analyze(frameInfo);
                snapshotHandler.Analyze(frameInfo);
                motionHandler.Analyze(frameInfo);
                eventDetectionHandler.Analyze(frameInfo);

                Thread.Sleep(40);
            }

            Assert.AreEqual(2, eventProcessor.LastEventTime.Count);
            Assert.True(eventProcessor.LastEventTime.Keys.Contains("F_person:1"));
            Assert.True(eventProcessor.LastEventTime.Keys.Contains("F_car:2"));

            Assert.AreEqual(1, eventPublisher.PublishedEventsByCategory.Count);
            Assert.AreEqual(2, eventPublisher.PublishedEventsByCategory["Forbidden"].Count);
        }

        [Test]
        public void TestGenerateStoppedEventDetector()
        {
            RoadDefinition roadDefinition = RoadDefinition.LoadFromJson("RoadDefinition/hc_k41_700_2.json");
            roadDefinition.SetImageSize(1920, 1080);
            roadDefinition.MotionCalculationFrameInterval = 5;
            roadDefinition.StopEventSpeedUpperLimit = 5;
            roadDefinition.StopEventEnableDurationSec = 5;

            RegionHandler regionHandler = new RegionHandler();
            regionHandler.SetRoadDefinition(roadDefinition);

            SnapshotHandler snapshotHandler = new SnapshotHandler();
            snapshotHandler.SetRoadDefinition(roadDefinition);

            LaneHandler laneHandler = new LaneHandler();
            laneHandler.SetRoadDefinition(roadDefinition);

            EventDetectionHandler eventDetectionHandler = new EventDetectionHandler(snapshotHandler.Service);
            eventDetectionHandler.SetRoadDefinition(roadDefinition);

            ITrafficEventGenerator generator = new SanbaoEventGenerator();
            EventProcessor eventProcessor = new EventProcessor(30, generator);

            ITrafficEventPublisher publisher = new SanboEventPublisher(_reportApiUri);
            EventPublisher eventPublisher = new EventPublisher(publisher);

            string captureRoot = @"D:\Capture";
            StoppedTypeEventAlg stoppedAlg = new StoppedTypeEventAlg(captureRoot, 1, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(stoppedAlg);

            ISpeeder speeder = new GeographySpeeder();
            MotionHandler motionHandler = new MotionHandler(speeder);
            motionHandler.SetRoadDefinition(roadDefinition);

            DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);
            for (int i = 1; i <= 10; i++)
            {
                string filename = $"Images/pl_0000{i:D2}.jpg";
                Mat mat = new Mat(filename, ImreadModes.Color);

                FrameInfo frameInfo = new FrameInfo(i, mat);
                string json = File.ReadAllText($"Json/pl_0000{i:D2}.json");
                frameInfo.ObjectInfos = JsonSerializer.Deserialize<List<TrafficObjectInfo>>(json);
                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    toi.FrameId = frameInfo.FrameId;
                    toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
                    toi.IsAnalyzable = true;
                }

                regionHandler.Analyze(frameInfo);
                laneHandler.Analyze(frameInfo);
                snapshotHandler.Analyze(frameInfo);
                motionHandler.Analyze(frameInfo);
                eventDetectionHandler.Analyze(frameInfo);

                Thread.Sleep(40);
            }

            Assert.AreEqual(1, eventProcessor.LastEventTime.Count);
            Assert.True(eventProcessor.LastEventTime.Keys.Contains("P_car:2"));

            Assert.AreEqual(1, eventPublisher.PublishedEventsByCategory.Count);
            Assert.AreEqual(1, eventPublisher.PublishedEventsByCategory["Stopped"].Count);
        }
    }
}
