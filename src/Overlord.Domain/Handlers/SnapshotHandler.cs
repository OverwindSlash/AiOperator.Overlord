using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;
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

        public override void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            base.SetRoadDefinition(roadDefinition);
            _snapshotService.SetRoadDefinition(roadDefinition);
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            _snapshotService.AddSceneByFrameId(frameInfo.FrameId, frameInfo.Scene);
            _snapshotService.AddSnapshotOfObjectById(frameInfo);

            return frameInfo;
        }

        public override void Dispose()
        {
            _snapshotService.Dispose();
        }
    }
}
