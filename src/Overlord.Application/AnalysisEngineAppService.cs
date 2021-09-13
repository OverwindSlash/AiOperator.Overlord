using Overlord.Domain;
using Overlord.Domain.EventManagerSanbao;
using Overlord.Domain.Geography;
using Overlord.Domain.Interfaces;
using Overlord.Domain.ObjectDetector;
using Overlord.Domain.ObjectDetector.Config;

namespace Overlord.Application
{
    public class AnalysisEngineAppService
    {
        private const int MaxPipelineCount = 8;
        private readonly DependencyRegister _dependencyRegister;
        private AnalysisEngine _engine;

        public AnalysisEngineAppService()
        {
            _dependencyRegister = DependencyRegister.GetInstance();
        }

        public void InitializeEngine(string appSettingsFile)
        {
            _dependencyRegister.Reset();
            
            string configPath = @"Model/coco";
            YoloConfiguration yoloConfig = ConfigurationLoader.Load(configPath);
            IObjectDetector detector = new YoloWrapper(yoloConfig);

            for (int i = 0; i < MaxPipelineCount; i++)
            {
                _dependencyRegister.AddObjectDetector(detector);
                _dependencyRegister.AddEventGenerator(new SanbaoEventGenerator());
                _dependencyRegister.AddEventPublisher(new SanboEventPublisher());
                _dependencyRegister.AddSpeeder(new GeographySpeeder());
            }

            _engine.Dispose();
            _engine = new AnalysisEngine(_dependencyRegister);
            _engine.LoadLaunchSettings(appSettingsFile);
        }
    }
}
