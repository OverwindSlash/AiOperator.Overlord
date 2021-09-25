using Overlord.Domain;
using Overlord.Domain.EventManagerSanbao;
using Overlord.Domain.Geography;
using Overlord.Domain.Interfaces;
using Overlord.Domain.ObjectDetector;
using Overlord.Domain.ObjectDetector.Config;
using Overlord.Domain.ObjectTracker;
using Overlord.Domain.Pipeline;
using Overlord.Domain.Settings;

namespace Overlord.Application
{
    public class AnalysisEngineAppService
    {
        private const int MaxPipelineCount = 8;
        private readonly DependencyRegister _dependencyRegister;
        private AnalysisEngine _engine;
        private ApplicationSettings _settings;

        public ApplicationSettings AppSettings => _settings;

        public AnalysisEngineAppService()
        {
            _dependencyRegister = DependencyRegister.GetInstance();
        }

        public void InitializeEngine(string appSettingFile)
        {
            _dependencyRegister.Reset();
            
            string configPath = @"Model/coco";
            YoloConfiguration yoloConfig = ConfigurationLoader.Load(configPath);
            IObjectDetector detector = new YoloWrapper(yoloConfig);

            for (int i = 0; i < MaxPipelineCount; i++)
            {
                _dependencyRegister.AddObjectDetector(detector);
                _dependencyRegister.AddMultiObjectTracker(new SortTracker());
                _dependencyRegister.AddEventGenerator(new SanbaoEventGenerator());
                _dependencyRegister.AddEventPublisher(new SanboEventPublisher(_settings.PipelineSettings[i].PublishApiUri));
                _dependencyRegister.AddSpeeder(new GeographySpeeder());
            }

            if (_engine != null)
            {
                _engine.Dispose();
            }
            
            _engine = new AnalysisEngine(_dependencyRegister);

            _settings = new ApplicationSettings();
            _settings.LoadFromJson(appSettingFile);
        }

        public AnalysisPipeline OpenAnalysisPipeline(PipelineSetting pipelineSetting)
        {
            return _engine.AddAndGetPipeline(pipelineSetting);
        }
    }
}
