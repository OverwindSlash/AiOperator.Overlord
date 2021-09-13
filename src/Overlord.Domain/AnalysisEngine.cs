using System;
using System.Collections.Generic;
using OpenCvSharp;
using Overlord.Domain.Event;
using Overlord.Domain.EventAlg;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using Overlord.Domain.Pipeline;
using Overlord.Domain.Settings;

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

        public void LoadLaunchSettings(string filename)
        {
            ApplicationSettings settings = new ApplicationSettings();
            settings.LoadFromJson(filename);

            int pipelineIndex = 0;
            foreach (PipelineSetting pipelineSetting in settings.PipelineSettings)
            {
                string videoUri = pipelineSetting.VideoSource;
                string roadDefFile = pipelineSetting.RoadDefinitionFile;
                
                var (width, height) = GetVideoWidthAndHeight(videoUri);

                AnalysisPipeline pipeline = new AnalysisPipeline();
                pipeline.LoadRoadDefinition(roadDefFile, width, height);

                IObjectDetector detector = _dependencyRegister.GetDetector(pipelineIndex);
                DetectionHandler detectionHandler = new DetectionHandler(detector);
                pipeline.AddAnalysisHandler(detectionHandler);
                
                RegionHandler regionHandler = new RegionHandler();
                pipeline.AddAnalysisHandler(regionHandler);
                
                SnapshotHandler snapshotHandler = new SnapshotHandler();
                pipeline.AddAnalysisHandler(snapshotHandler);

                LaneHandler laneHandler = new LaneHandler();
                pipeline.AddAnalysisHandler(laneHandler);
                
                EventDetectionHandler eventDetectionHandler = new EventDetectionHandler(snapshotHandler.Service);
                pipeline.AddAnalysisHandler(eventDetectionHandler);

                ITrafficEventGenerator generator = _dependencyRegister.GetEventGenerator(pipelineIndex);
                EventProcessor eventProcessor = new EventProcessor(settings.MinTriggerIntervalSecs, generator);

                ITrafficEventPublisher publisher = _dependencyRegister.GetEventPublisher(pipelineIndex);
                EventPublisher eventPublisher = new EventPublisher(publisher);

                string captureRoot = @"D:\Capture";
                StoppedTypeEventAlg stoppedAlg = new StoppedTypeEventAlg(captureRoot, 1, eventProcessor, eventPublisher);
                eventDetectionHandler.AddEventAlgorithm(stoppedAlg);

                ISpeeder speeder = _dependencyRegister.GetSpeeder(pipelineIndex);
                MotionHandler motionHandler = new MotionHandler(speeder);
                pipeline.AddAnalysisHandler(motionHandler);

                pipelineIndex++;
            }
        }

        private static (int width, int height) GetVideoWidthAndHeight(string videoUri)
        {
            var capture = new VideoCapture(videoUri, VideoCaptureAPIs.FFMPEG);
            if (!capture.IsOpened())
            {
                // TODO: Report error!
                return (0, 0);
            }

            int width = capture.FrameWidth;
            int height = capture.FrameHeight;
            return (width, height);
        }

        public void AddPipeline(AnalysisPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentException("analysis pipeline is null");
            }

            if (_pipelines.Contains(pipeline))
            {
                return;
            }
            
            _pipelines.Add(pipeline);
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