using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Geometric;
using Overlord.Core.Entities.Road;
using Overlord.Domain.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Overlord.Domain.Services
{
    public class RegionService : IObserver<ObjectExpiredEvent>
    {
        private RoadDefinition _roadDefinition;
        private bool _isObjectAnalyzableRetain;
        private List<AnalysisArea> _analysisAreas;
        private List<ExcludedArea> _excludedAreas;

        // Id(type:trackingId) -> trakingId
        private readonly ConcurrentDictionary<string, long> _allTrackingIdsUnderAnalysis;

        public RegionService()
        {
            _allTrackingIdsUnderAnalysis = new ConcurrentDictionary<string, long>();
        }

        public void SetRoadDefinition(RoadDefinition roadDefinition)
        {
            _allTrackingIdsUnderAnalysis.Clear();
            _roadDefinition = roadDefinition;
            _isObjectAnalyzableRetain = roadDefinition.IsObjectAnalyzableRetain;
            _analysisAreas = roadDefinition.AnalysisAreas;
            _excludedAreas = roadDefinition.ExcludedAreas;
        }

        public void DetermindAnalysisObjects(List<TrafficObjectInfo> tois)
        {
            foreach (TrafficObjectInfo toi in tois)
            {
                if (_isObjectAnalyzableRetain && _allTrackingIdsUnderAnalysis.ContainsKey(toi.Id))
                {
                    toi.IsAnalyzable = false;
                    continue;
                }

                NormalizedPoint point = new NormalizedPoint(_roadDefinition.ImageWidth, _roadDefinition.ImageHeight,
                    toi.CenterX, toi.CenterY);

                foreach (AnalysisArea analysisArea in _analysisAreas)
                {
                    if (analysisArea.IsPointInPolygon(point))
                    {
                        toi.IsAnalyzable = true;
                        break;
                    }
                }

                foreach (ExcludedArea excludedArea in _excludedAreas)
                {
                    if (excludedArea.IsPointInPolygon(point))
                    {
                        toi.IsAnalyzable = false;
                        break;
                    }
                }

                if (toi.IsAnalyzable)
                {
                    _allTrackingIdsUnderAnalysis.TryAdd(toi.Id, toi.TrackingId);
                }
            }
        }

        public int GetAnalyzableObjectCount()
        {
            return _allTrackingIdsUnderAnalysis.Count;
        }

        public void OnCompleted()
        {
            // Do nothing
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public void OnNext(ObjectExpiredEvent value)
        {
            Task.Run(() =>
            {
                ReleaseAnalyzableObjectById(value.Id);
            });

        }

        public void ReleaseAnalyzableObjectById(string id)
        {
            if (_allTrackingIdsUnderAnalysis.ContainsKey(id))
            {
                _allTrackingIdsUnderAnalysis.TryRemove(id, out var value);
            }
        }
    }
}
