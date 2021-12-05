using OpenCvSharp;
using Overlord.Domain.Event;
using Overlord.Domain.EventAlg;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using Overlord.Domain.Pipeline;
using Overlord.Domain.Settings;
using System;
using System.Collections.Generic;

namespace Overlord.Domain
{
    public class AnalysisEngine : IDisposable
    {
        private readonly DependencyRegister _dependencyRegister;
        private readonly List<AnalysisPipeline> _pipelines;

        public AnalysisEngine(DependencyRegister dependencyRegister)
        {
            _dependencyRegister = dependencyRegister;
            _pipelines = new List<AnalysisPipeline>();
        }

        public AnalysisPipeline AddAndGetPipeline(PipelineSetting settings)
        {
            AnalysisPipeline pipeline = new AnalysisPipeline();

            pipeline.Name = settings.Name;
            pipeline.Fps = settings.Fps;

            pipeline.VideoUri = settings.VideoUri;
            var (width, height) = GetVideoWidthAndHeight(pipeline.VideoUri);

            pipeline.RoadDefinitionFile = settings.RoadDefinitionFile;
            pipeline.LoadRoadDefinition(pipeline.RoadDefinitionFile, width, height);

            _pipelines.Add(pipeline);;

            InitializeDefaultHandlers(pipeline, _pipelines.IndexOf(pipeline), settings);

            return pipeline;
        }

        private static (int width, int height) GetVideoWidthAndHeight(string videoUri)
        {
            using var capture = new VideoCapture(videoUri, VideoCaptureAPIs.FFMPEG);
            if (!capture.IsOpened())
            {
                throw new ArgumentException($"can not open video uri: {videoUri}");
            }

            int width = capture.FrameWidth;
            int height = capture.FrameHeight;
            return (width, height);
        }

        private void InitializeDefaultHandlers(AnalysisPipeline pipeline, int pipelineIndex, PipelineSetting settings)
        {
            // Detection
            IObjectDetector detector = _dependencyRegister.GetDetector(pipelineIndex);
            DetectionHandler detectionHandler = new DetectionHandler(detector);
            pipeline.AddAnalysisHandler(detectionHandler);

            // Region
            RegionHandler regionHandler = new RegionHandler();
            pipeline.AddAnalysisHandler(regionHandler);

            // Tracking
            IMultiObjectTracker tracker = _dependencyRegister.GetTracker(pipelineIndex);
            TrackingHandler trackingHandler = new TrackingHandler(tracker);
            pipeline.AddAnalysisHandler(trackingHandler);

            // Snapshot
            SnapshotHandler snapshotHandler = new SnapshotHandler();
            pipeline.AddAnalysisHandler(snapshotHandler);
            pipeline.Subscribe((IObserver<ObjectExpiredEvent>)snapshotHandler.Service);
            pipeline.Subscribe((IObserver<FrameExpiredEvent>)snapshotHandler.Service);

            // Lane
            LaneHandler laneHandler = new LaneHandler();
            pipeline.AddAnalysisHandler(laneHandler);

            // Motion
            ISpeeder speeder = _dependencyRegister.GetSpeeder(pipelineIndex);
            MotionHandler motionHandler = new MotionHandler(speeder);
            pipeline.AddAnalysisHandler(motionHandler);
            pipeline.Subscribe(motionHandler.Service);

            // Counting
            CountingHandler countingHandler = new CountingHandler();
            pipeline.AddAnalysisHandler(countingHandler);
            pipeline.Subscribe(countingHandler.Service);

            // Event Detection
            EventDetectionHandler eventDetectionHandler = new EventDetectionHandler(snapshotHandler.Service);
            pipeline.AddAnalysisHandler(eventDetectionHandler);

            ITrafficEventGenerator generator = _dependencyRegister.GetEventGenerator(pipelineIndex);
            EventProcessor eventProcessor = new EventProcessor(settings.MinTriggerIntervalSecs, generator);
            pipeline.Subscribe(eventProcessor);

            ITrafficEventPublisher publisher = _dependencyRegister.GetEventPublisher(pipelineIndex);
            EventPublisher eventPublisher = new EventPublisher(publisher);
            pipeline.Subscribe(eventPublisher);

            string captureRoot = settings.CaptureRoot;

            StoppedTypeEventAlg stoppedAlg = new StoppedTypeEventAlg(captureRoot, settings.Fps, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(stoppedAlg);
            pipeline.Subscribe(stoppedAlg);

            SlowVehicleEventAlg slowVehicleEventAlg = new SlowVehicleEventAlg(captureRoot, settings.Fps, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(slowVehicleEventAlg);
            pipeline.Subscribe(slowVehicleEventAlg);

            ForbiddenTypeEventAlg forbiddenAlg = new ForbiddenTypeEventAlg(captureRoot, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(forbiddenAlg);
            pipeline.Subscribe(forbiddenAlg);
        }

        public AnalysisPipeline GetPipelineByIndex(int pipelineIndex)
        {
            if ((pipelineIndex > _pipelines.Count - 1) || (pipelineIndex < 0))
            {
                throw new ArgumentException("invalid pipeline index.");
            }

            return _pipelines[pipelineIndex];
        }

        public void Dispose()
        {
            foreach (AnalysisPipeline pipeline in _pipelines)
            {
                pipeline.Dispose();
            }
        }
    }
}