using Overlord.Core.Entities.Frame;
using Overlord.Domain.Services;

namespace Overlord.Domain.Handlers
{
    public class SnapshotHandler : AnalysisHandlerBase
    {
        private readonly SnapshotService _snapshotService;

        public SnapshotHandler()
        {
            _snapshotService = new SnapshotService();
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            _snapshotService.AddSceneByFrameId(frameInfo.FrameId, frameInfo.Scene);
            return frameInfo;
        }
    }
}
