using NSubstitute;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using Overlord.Domain.Pipeline;
using System;
using System.Collections.Generic;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class AnalysisPipelineTests
    {
        private const int ImageWidth = 1920;
        private const int ImageHeight = 1080;

        [Test]
        public void TestAnalysisPipelineGeneration_WithoutCorrectRoadDefinition()
        {
            AnalysisPipeline pipeline = new AnalysisPipeline();
            Assert.Catch<TypeInitializationException>(() =>
            {
                FrameInfo frameInfo = new FrameInfo(0L, new Mat());
                pipeline.Analyze(frameInfo);
            });
        }

        [Test]
        public void TestAnalysisPipelineGeneration_WithCorrectRoadDefinition()
        {
            AnalysisPipeline pipeline = new AnalysisPipeline();
            pipeline.LoadRoadDefinition("RoadDefinition/demoRd.json", ImageWidth, ImageHeight);

            Assert.AreEqual(2, pipeline.RoadDef.AnalysisAreas.Count);
            Assert.AreEqual(2, pipeline.RoadDef.ExcludedAreas.Count);
            Assert.AreEqual(5, pipeline.RoadDef.Lanes.Count);
            Assert.AreEqual(1, pipeline.RoadDef.CountLines.Count);

            Assert.AreEqual(ImageWidth, pipeline.RoadDef.AnalysisAreas[0].ImageWidth);
            Assert.AreEqual(ImageHeight, pipeline.RoadDef.AnalysisAreas[0].ImageHeight);

            using Mat mat = new Mat("Images/Traffic_001.jpg", ImreadModes.Color);
            FrameInfo frameInfo = new FrameInfo(0L, mat);
            FrameInfo result = pipeline.Analyze(frameInfo);
            Assert.AreSame(frameInfo, result);
        }

        [Test]
        public void TestAnalysisPipeline_AddHandler()
        {
            AnalysisPipeline pipeline = new AnalysisPipeline();
            pipeline.LoadRoadDefinition("RoadDefinition/demoRd.json", ImageWidth, ImageHeight);

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
            pipeline.AddAnalysisHandler(handler);

            FrameInfo frameInfo = new FrameInfo(0L, mat);
            pipeline.Analyze(frameInfo);
        }
    }
}
