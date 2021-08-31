using Overlord.Core.Entities.Geometric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Overlord.Core.Entities.Road
{
    public class RoadDefinition : ImageBasedGeometric
    {
        private List<AnalysisArea> _analysisAreas;
        private List<ExcludedArea> _excludedAreas;
        private List<Lane> _lanes;
        private List<Tuple<EnterLine, LeaveLine>> _countLines;

        public string RoadName { get; set; }
        public string DeviceNo { get; set; }
        public string PrePositionName { get; set; }
        public float DetectionThresh { get; set; }
        public bool IsObjectAnalyzableRetain { get; set; }
        public bool TrackingChangeHistory { get; set; }
        public int TrackingFramesStory { get; set; }
        public int TrackingMaxDistance { get; set; }

        public List<AnalysisArea> AnalysisAreas
        {
            get
            {
                CheckImageSizeInitialized();
                return _analysisAreas;
            }
            set => _analysisAreas = value;
        }

        public List<ExcludedArea> ExcludedAreas
        {
            get
            {
                CheckImageSizeInitialized();
                return _excludedAreas;
            }
            set => _excludedAreas = value;
        }

        public List<Lane> Lanes
        {
            get
            {
                CheckImageSizeInitialized();
                return _lanes;
            }
            set => _lanes = value;
        }

        public List<Tuple<EnterLine, LeaveLine>> CountLines
        {
            get
            {
                CheckImageSizeInitialized();
                return _countLines;
            }
            set => _countLines = value;
        }

        public RoadDefinition()
        {
            AnalysisAreas = new List<AnalysisArea>();
            ExcludedAreas = new List<ExcludedArea>();
            Lanes = new List<Lane>();
            CountLines = new List<Tuple<EnterLine, LeaveLine>>();

            DetectionThresh = 0.7f;
            TrackingChangeHistory = true;
            TrackingFramesStory = 50;
            TrackingMaxDistance = 40;
        }
        
        public override void SetImageSize(int width, int height)
        {
            base.SetImageSize(width, height);
            
            foreach (var area in AnalysisAreas)
            {
                area.SetImageSize(width, height);
            }

            foreach (var area in ExcludedAreas)
            {
                area.SetImageSize(width, height);
            }

            foreach (var lane in Lanes)
            {
                lane.SetImageSize(width, height);
            }

            foreach (var countLine in CountLines)
            {
                countLine.Item1.SetImageSize(width, height);
                countLine.Item2.SetImageSize(width, height);
            }
        }

        public void AddAnalysisArea(AnalysisArea analysisArea)
        {
            ValidateImageWidthAndHeight(analysisArea);
            AnalysisAreas.Add(analysisArea);
        }

        public void AddExcludedArea(ExcludedArea excludedArea)
        {
            ValidateImageWidthAndHeight(excludedArea);
            ExcludedAreas.Add(excludedArea);
        }

        public void AddLane(Lane lane)
        {
            ValidateImageWidthAndHeight(lane);
            Lanes.Add(lane);
        }

        public void AddCountLinePair(EnterLine enterLine, LeaveLine leaveLine)
        {
            ValidateImageWidthAndHeight(enterLine);
            ValidateImageWidthAndHeight(leaveLine);
            CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
        }
        
        private void ValidateImageWidthAndHeight(NormalizedPolygon poiRegion)
        {
            if (!IsInitialized())
            {
                base.SetImageSize(poiRegion.ImageWidth, poiRegion.ImageHeight);
            }

            if ((poiRegion.ImageWidth != ImageWidth) || (poiRegion.ImageHeight != ImageHeight))
            {
                throw new ArgumentException($"poi region does not match scale of others.");
            }
        }
        
        private void ValidateImageWidthAndHeight(NormalizedLine poiLine)
        {
            if (!IsInitialized())
            {
                base.SetImageSize(poiLine.ImageWidth, poiLine.ImageHeight);
            }

            if ((poiLine.ImageWidth != ImageWidth) || (poiLine.ImageHeight != ImageHeight))
            {
                throw new ArgumentException($"poi line does not match scale of others.");
            }
        }

        public static void SaveToJson(string filename, RoadDefinition roadDefinition)
        {
            string jsonString = JsonSerializer.Serialize(roadDefinition);
            File.WriteAllText(filename, jsonString);
        }

        public static RoadDefinition LoadFromJson(string filename)
        {
            string jsonString = File.ReadAllText(filename);
            var roadDefinition = JsonSerializer.Deserialize<RoadDefinition>(jsonString);
            return roadDefinition;
        }
    }
}