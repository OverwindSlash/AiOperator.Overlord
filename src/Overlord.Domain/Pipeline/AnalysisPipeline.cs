using System;
using System.Collections.Generic;
using System.IO;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using Overlord.Domain.EventAlg;
using Overlord.Domain.Handlers;
using Overlord.Domain.Interfaces;
using Overlord.Domain.Services;
using Overlord.Domain.Settings;

namespace Overlord.Domain.Pipeline
{
    public class AnalysisPipeline : IDisposable, IObservable<ObjectExpiredEvent>, IObservable<FrameExpiredEvent>
    {
        private const int DefaultFrameLifeTime = 125;
        private readonly List<IAnalysisHandler> _analysisHandlers;
        private readonly FrameLifeTimeManager _frameLifeTimeManager;
        private readonly DependencyRegister _dependencyRegister;

        private RoadDefinition _roadDefinition;
        private int _imageWidth;
        private int _imageHeight;
        private bool _initialized;
        private SnapshotService _snapshotService;

        public PipelineSettings Settingses { get; set; }
        public string Name => Settingses.Name;
        public string VideoUri => Settingses.VideoUri;
        public int Fps => Settingses.Fps;
        public string RoadDefinitionFile => Settingses.RoadDefinitionFile;

        public RoadDefinition RoadDef => _roadDefinition;
        public int ImageWidth => _imageWidth;
        public int ImageHeight => _imageHeight;

        public FrameLifeTimeManager LifeTimeManager => _frameLifeTimeManager;

        public AnalysisPipeline(DependencyRegister dependencyRegister)
        {
            _analysisHandlers = new List<IAnalysisHandler>();
            _frameLifeTimeManager = new FrameLifeTimeManager(DefaultFrameLifeTime);
            _initialized = false;
            _dependencyRegister = dependencyRegister;
        }

        public void LoadRoadDefinition(string roadDefinitionFile, int imageWidth, int imageHeight)
        {
            if ((imageWidth == 0) || (imageHeight == 0))
            {
                throw new ArgumentException("image width and height not set.");
            }
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;

            if (!File.Exists(roadDefinitionFile))
            {
                throw new ArgumentException("road definition file not found.");
            }
            _roadDefinition = RoadDefinition.LoadFromJson(roadDefinitionFile);
            if (_roadDefinition == null)
            {
                throw new ArgumentException("road definition file corrupted.");
            }

            _roadDefinition.SetImageSize(imageWidth, imageHeight);
            _initialized = true;
        }
        
        public void InitializeDefaultHandlers(int pipelineIndex)
        {
            // Detection
            IObjectDetector detector = _dependencyRegister.GetDetector(pipelineIndex);
            DetectionHandler detectionHandler = new DetectionHandler(detector);
            this.AddAnalysisHandler(detectionHandler);

            // Region
            RegionHandler regionHandler = new RegionHandler();
            this.AddAnalysisHandler(regionHandler);
            this.Subscribe(regionHandler.Service);

            // Tracking
            IMultiObjectTracker tracker = _dependencyRegister.GetTracker(pipelineIndex);
            TrackingHandler trackingHandler = new TrackingHandler(tracker);
            this.AddAnalysisHandler(trackingHandler);

            // Snapshot
            SnapshotHandler snapshotHandler = new SnapshotHandler();
            this.AddAnalysisHandler(snapshotHandler);
            this.Subscribe((IObserver<ObjectExpiredEvent>)snapshotHandler.Service);
            this.Subscribe((IObserver<FrameExpiredEvent>)snapshotHandler.Service);
            this._snapshotService = snapshotHandler.Service;

            // Lane
            LaneHandler laneHandler = new LaneHandler();
            this.AddAnalysisHandler(laneHandler);

            // Motion
            ISpeeder speeder = _dependencyRegister.GetSpeeder(pipelineIndex);
            MotionHandler motionHandler = new MotionHandler(speeder);
            this.AddAnalysisHandler(motionHandler);
            this.Subscribe(motionHandler.Service);

            // Counting
            CountingHandler countingHandler = new CountingHandler();
            this.AddAnalysisHandler(countingHandler);
            this.Subscribe(countingHandler.Service);

            // Event Detection
            EventDetectionHandler eventDetectionHandler = new EventDetectionHandler(this._snapshotService);
            this.AddAnalysisHandler(eventDetectionHandler);

            ITrafficEventGenerator generator = _dependencyRegister.GetEventGenerator(pipelineIndex);
            EventProcessor eventProcessor = new EventProcessor(Settingses.MinTriggerIntervalSecs, generator);
            this.Subscribe(eventProcessor);

            ITrafficEventPublisher publisher = _dependencyRegister.GetEventPublisher(pipelineIndex);
            EventPublisher eventPublisher = new EventPublisher(publisher);
            this.Subscribe(eventPublisher);

            string captureRoot = Settingses.CaptureRoot;

            StoppedTypeEventAlg stoppedAlg = new StoppedTypeEventAlg(captureRoot, Settingses.Fps, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(stoppedAlg);
            this.Subscribe(stoppedAlg);

            SlowVehicleEventAlg slowVehicleEventAlg = new SlowVehicleEventAlg(captureRoot, Settingses.Fps, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(slowVehicleEventAlg);
            this.Subscribe(slowVehicleEventAlg);

            ForbiddenTypeEventAlg forbiddenAlg = new ForbiddenTypeEventAlg(captureRoot, eventProcessor, eventPublisher);
            eventDetectionHandler.AddEventAlgorithm(forbiddenAlg);
            this.Subscribe(forbiddenAlg);
        }
        
        public void AddAnalysisHandler(IAnalysisHandler handler)
        {
            if (handler == null)
            {
                return;
            }

            if (_analysisHandlers.Contains(handler))
            {
                return;
            }

            handler.SetRoadDefinition(RoadDef);
            _analysisHandlers.Add(handler);
        }

        public void RemoveAnalysisHandler(IAnalysisHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            
            if (!_analysisHandlers.Contains(handler))
            {
                return;
            }

            _analysisHandlers.Remove(handler);
        }

        public FrameInfo Analyze(FrameInfo frameInfo)
        {
            if (!_initialized)
            {
                throw new TypeInitializationException(this.GetType().FullName,
                    new ArgumentException("pipeline not initialized correctly."));
            }

            foreach (IAnalysisHandler handler in _analysisHandlers)
            {
                frameInfo = handler.Analyze(frameInfo);
            }

            _frameLifeTimeManager.AddFrameInfo(frameInfo);

            return frameInfo;
        }

        public IDisposable Subscribe(IObserver<ObjectExpiredEvent> observer)
        {
            return _frameLifeTimeManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<FrameExpiredEvent> observer)
        {
            return _frameLifeTimeManager.Subscribe(observer);
        }

        public void Dispose()
        {
            foreach (IAnalysisHandler handler in _analysisHandlers)
            {
                handler.Dispose();
            }
        }
    }
}
