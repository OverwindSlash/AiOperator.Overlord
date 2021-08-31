using Overlord.Core.Entities.Frame;
using Overlord.Domain.Services;

namespace Overlord.Domain.Handlers
{
    public class SnapshotHandler : AnalysisHandlerBase
    {
        private readonly SnapshotService _snapshotService;

        public SnapshotService Service => _snapshotService;

        public SnapshotHandler()
        {
            _snapshotService = new SnapshotService();
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            Service.AddSceneByFrameId(frameInfo.FrameId, frameInfo.Scene);
            return frameInfo;
        }

        public override void Dispose()
        {
            Service.Dispose();
        }
    }
}
