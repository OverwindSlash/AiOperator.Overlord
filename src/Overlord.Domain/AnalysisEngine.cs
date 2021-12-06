using System;
using System.Collections.Generic;
using OpenCvSharp;
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

        public AnalysisPipeline AddAndGetPipeline(PipelineSettings pipelineSettings)
        {
            AnalysisPipeline pipeline = new AnalysisPipeline(_dependencyRegister);

            pipeline.Settingses = pipelineSettings;
            var (width, height) = GetVideoWidthAndHeight(pipelineSettings.VideoUri);

            pipeline.LoadRoadDefinition(pipeline.RoadDefinitionFile, width, height);
            
            _pipelines.Add(pipeline);
            
            // TODO: change search dependencies from by id to by name 
            pipeline.InitializeDefaultHandlers(_pipelines.IndexOf(pipeline));

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