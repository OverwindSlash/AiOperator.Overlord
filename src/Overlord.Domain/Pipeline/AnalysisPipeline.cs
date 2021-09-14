using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using Overlord.Domain.Event;

namespace Overlord.Domain.Pipeline
{
    public class AnalysisPipeline : IDisposable, IObservable<ObjectExpiredEvent>, IObservable<FrameExpiredEvent>
    {
        private const int DefaultFrameCount = 50;
        private readonly List<IAnalysisHandler> _analysisHandlers;
        private readonly FrameLifeTimeManager _frameLifeTimeManager;

        private RoadDefinition _roadDefinition;
        private int _imageWidth;
        private int _imageHeight;
        private bool _initialized;

        public string Name { get; set; }
        public string VideoUri { get; set; }
        public int Fps { get; set; }
        public string RoadDefinitionFile { get; set; }

        public RoadDefinition RoadDef => _roadDefinition;
        public int ImageWidth => _imageWidth;
        public int ImageHeight => _imageHeight;

        public FrameLifeTimeManager LifeTimeManager => _frameLifeTimeManager;


        public AnalysisPipeline()
        {
            _analysisHandlers = new List<IAnalysisHandler>();
            _frameLifeTimeManager = new FrameLifeTimeManager(DefaultFrameCount);
            _initialized = false;
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
