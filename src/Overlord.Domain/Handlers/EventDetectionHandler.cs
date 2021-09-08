using Overlord.Core.Entities.Frame;
using Overlord.Domain.EventAlg;
using Overlord.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Overlord.Domain.Handlers
{
    public class EventDetectionHandler : AnalysisHandlerBase
    {
        private readonly SnapshotService _snapshotService;
        private readonly List<IEventAlgorithm> _eventAlgorithms;

        public List<IEventAlgorithm> EventAlgorithms => _eventAlgorithms;

        public EventDetectionHandler(SnapshotService snapshotService)
        {
            _snapshotService = snapshotService;
            _eventAlgorithms = new List<IEventAlgorithm>();
        }

        public void AddEventAlgorithm(IEventAlgorithm eventAlgorithm)
        {
            if (eventAlgorithm == null)
            {
                throw new ArgumentException("event algorithm is null.");
            }

            if (!_eventAlgorithms.Contains(eventAlgorithm))
            {
                _eventAlgorithms.Add(eventAlgorithm);
                eventAlgorithm.SetRoadDefinition(_roadDefinition);
                eventAlgorithm.SetSnapshotService(_snapshotService);
            }
        }

        public void RemoveEventAlgorithm(IEventAlgorithm eventAlgorithm)
        {
            if (eventAlgorithm == null)
            {
                throw new ArgumentException("event algorithm is null.");
            }

            if (_eventAlgorithms.Contains(eventAlgorithm))
            {
                _eventAlgorithms.Remove(eventAlgorithm);
            }
        }

        public override FrameInfo Analyze(FrameInfo frameInfo)
        {
            foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
            {
                Parallel.ForEach(_eventAlgorithms, alg => alg.DetectEvent(toi, frameInfo));
            }

            return frameInfo;
        }

        public override void Dispose()
        {
            foreach (IEventAlgorithm algorithm in _eventAlgorithms)
            {
                algorithm.Dispose();
            }
        }
    }
}
