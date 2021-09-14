using NUnit.Framework;

namespace Overlord.Application.Tests
{
    public class AnalysisEngineAppServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGenerateAnalysisEngineAppService()
        {
            AnalysisEngineAppService engineAppService = new AnalysisEngineAppService();

            engineAppService.InitializeEngine("appSettings.json");
        }
    }
} 