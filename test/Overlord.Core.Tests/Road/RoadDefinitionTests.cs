using NUnit.Framework;
using Overlord.Core.Entities.Geometric;
using Overlord.Core.Entities.Road;
using System;
using System.Text.Json;

namespace Overlord.Core.Tests.Road
{
    [TestFixture]
    public class RoadDefinitionTests
    {
        private const int ImageWidth = 1920;
        private const int ImageHeight = 1080;

        private string rdJson = "{\"RoadName\":\"hc_k41_700_1\",\"PrePositionName\":\"prepos0\",\"DetectionThresh\":0.7,\"IsObjectAnalyzableRetain\":false,\"TrackingChangeHistory\":true,\"TrackingFramesStory\":50,\"TrackingMaxDistance\":40," + 
                                "\"AnalysisAreas\":[" +
                                "{\"Name\":\"driveway region\",\"Points\":[" +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.6462962962962963}," +
                                "{\"NormalizedX\":0.5734375,\"NormalizedY\":0.4027777777777778}," +
                                "{\"NormalizedX\":0.7255208333333333,\"NormalizedY\":0.40185185185185185}," +
                                "{\"NormalizedX\":0.7182291666666667,\"NormalizedY\":1},{\"NormalizedX\":0,\"NormalizedY\":1}]}," +
                                "{\"Name\":\"ramp region\",\"Points\":[{\"NormalizedX\":0,\"NormalizedY\":0.41759259259259257}," +
                                "{\"NormalizedX\":0.17864583333333334,\"NormalizedY\":0.2518518518518518}," +
                                "{\"NormalizedX\":0.24635416666666668,\"NormalizedY\":0.3138888888888889}," +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.4675925925925926}]}],\"ExcludedAreas\":[" +
                                "{\"Name\":\"warn sign region\",\"Points\":[" +
                                "{\"NormalizedX\":0.5630208333333333,\"NormalizedY\":0.38055555555555554}," +
                                "{\"NormalizedX\":0.5901041666666667,\"NormalizedY\":0.38055555555555554}," +
                                "{\"NormalizedX\":0.5890625,\"NormalizedY\":0.42962962962962964}," +
                                "{\"NormalizedX\":0.5630208333333333,\"NormalizedY\":0.42962962962962964}]}],\"Lanes\":[" +
                                "{\"Name\":\"downward emergency lane\",\"Index\":1,\"Type\":1,\"ForbiddenTypes\":[\"person\",\"bicycle\",\"motorbike\",\"car\",\"bus\",\"truck\",\"train\"],\"Points\":[" +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.6462962962962963}," +
                                "{\"NormalizedX\":0.44114583333333335,\"NormalizedY\":0.4583333333333333}," +
                                "{\"NormalizedX\":0.4473958333333333,\"NormalizedY\":0.4722222222222222}," +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.7027777777777777}]}," +
                                "{\"Name\":\"downward driveway lane\",\"Index\":2,\"Type\":0,\"ForbiddenTypes\":[\"person\",\"bicycle\",\"motorbike\"],\"Points\":[" +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.7027777777777777}," +
                                "{\"NormalizedX\":0.5911458333333334,\"NormalizedY\":0.40370370370370373}," +
                                "{\"NormalizedX\":0.6416666666666667,\"NormalizedY\":0.40370370370370373}," +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.9101851851851852}]}," +
                                "{\"Name\":\"upward driveway lane\",\"Index\":3,\"Type\":0,\"ForbiddenTypes\":[\"person\",\"bicycle\",\"motorbike\"],\"Points\":[" +
                                "{\"NormalizedX\":0,\"NormalizedY\":0.9101851851851852}," +
                                "{\"NormalizedX\":0.6416666666666667,\"NormalizedY\":0.40370370370370373}," +
                                "{\"NormalizedX\":0.68125,\"NormalizedY\":0.40370370370370373}," +
                                "{\"NormalizedX\":0.6286458333333333,\"NormalizedY\":0.5305555555555556}," +
                                "{\"NormalizedX\":0.634375,\"NormalizedY\":0.5305555555555556}," +
                                "{\"NormalizedX\":0.7244791666666667,\"NormalizedY\":0.44074074074074077}," +
                                "{\"NormalizedX\":0.7244791666666667,\"NormalizedY\":0.4787037037037037}," +
                                "{\"NormalizedX\":0.696875,\"NormalizedY\":0.5083333333333333}," +
                                "{\"NormalizedX\":0.584375,\"NormalizedY\":1}]}," +
                                "{\"Name\":\"upward emergency lane\",\"Index\":4,\"Type\":1,\"ForbiddenTypes\":[\"person\",\"bicycle\",\"motorbike\",\"car\",\"bus\",\"truck\",\"train\",\"traffic light\"],\"Points\":[" +
                                "{\"NormalizedX\":0.584375,\"NormalizedY\":1}," +
                                "{\"NormalizedX\":0.696875,\"NormalizedY\":0.5083333333333333}," +
                                "{\"NormalizedX\":0.7244791666666667,\"NormalizedY\":0.4787037037037037}," +
                                "{\"NormalizedX\":0.7182291666666667,\"NormalizedY\":1}]}," +
                                "{\"Name\":\"upward ramp lane\",\"Index\":5,\"Type\":2,\"ForbiddenTypes\":[\"bicycle\",\"motorbike\"],\"Points\":[" +
                                "{\"NormalizedX\":0.3671875,\"NormalizedY\":0.6666666666666666}," +
                                "{\"NormalizedX\":0.5682291666666667,\"NormalizedY\":0.3611111111111111}," +
                                "{\"NormalizedX\":0.6354166666666666,\"NormalizedY\":0.30925925925925923}," +
                                "{\"NormalizedX\":0.5890625,\"NormalizedY\":0.6666666666666666}]}],\"CountLines\":[{\"Item1\":{\"Name\":\"upward driveway lane count line (enter)\",\"Start\":" +
                                "{\"NormalizedX\":0.14010416666666667,\"NormalizedY\":0.8324074074074074},\"Stop\":" +
                                "{\"NormalizedX\":0.7192708333333333,\"NormalizedY\":0.8888888888888888}},\"Item2\":{\"Name\":\"upward driveway lane count line (leave)\",\"Start\":" +
                                "{\"NormalizedX\":0.3463541666666667,\"NormalizedY\":0.6601851851851852},\"Stop\":" +
                                "{\"NormalizedX\":0.7213541666666666,\"NormalizedY\":0.7027777777777777}}}]}";
        
        [Test]
        public void Test_RoadDefinition_SaveMode()
        {
            RoadDefinition roadDefinition = new RoadDefinition();

            roadDefinition.RoadName = "hc_k41_700_1";
            roadDefinition.PrePositionName = "prepos0";

            // analysis param
            roadDefinition.DetectionThresh = 0.7F;
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            {
                // analysis area
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 698));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1101, 435));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1393, 434));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1379, 1080));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 1080));
                analysisArea.Name = "driveway region";
                roadDefinition.AddAnalysisArea(analysisArea);
            }

            {
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 451));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 343, 272));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 473, 339));
                analysisArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 505));
                analysisArea.Name = "ramp region";
                roadDefinition.AddAnalysisArea(analysisArea);
            }

            {
                // excluded area
                ExcludedArea excludedArea = new ExcludedArea();
                excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1081, 411));
                excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1133, 411));
                excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1131, 464));
                excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1081, 464));
                excludedArea.Name = "warn sign region";
                roadDefinition.AddExcludedArea(excludedArea);
            }

            {
                // excluded area
                // ExcludedArea excludedArea = new ExcludedArea();
                // excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1000, 900));
                // excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1100, 900));
                // excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1100, 1080));
                // excludedArea.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1000, 1080));
                // excludedArea.Name = "warn sign region";
                // roadDefinition.AddExcludedArea(excludedArea);
            }

            {
                // lane 1
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 698));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 847, 495));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 859, 510));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 759));
                lane.Name = "downward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 1;
                
                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 2
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 759));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1135, 436));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1232, 436));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 983));
                lane.Name = "downward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 2;
                
                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 3
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 0, 983));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1232, 436));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1308, 436));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1207, 573));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1218, 573));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1391, 476));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1391, 517));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1338, 549));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1122, 1080));
                lane.Name = "upward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 3;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 4
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1122, 1080));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1338, 549));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1391, 517));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1379, 1080));
                lane.Name = "upward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 4;

                lane.ForbiddenTypes.Add("traffic light");

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 5
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 705, 720));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1091, 390));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1220, 334));
                lane.AddPoint(new NormalizedPoint(ImageWidth, ImageHeight, 1131, 720));
                lane.Name = "upward ramp lane";
                lane.Type = LaneType.RampLane;
                lane.Index = 5;
                
                lane.ForbiddenTypes.Remove("person");

                roadDefinition.Lanes.Add(lane);
            }

            // count line
            EnterLine enterLine = new EnterLine();
            enterLine.Start = new NormalizedPoint(ImageWidth, ImageHeight, 269, 899);
            enterLine.Stop = new NormalizedPoint(ImageWidth, ImageHeight, 1381, 960);
            enterLine.Name = "upward driveway lane count line (enter)";

            LeaveLine leaveLine = new LeaveLine();
            leaveLine.Start = new NormalizedPoint(ImageWidth, ImageHeight, 665, 713);
            leaveLine.Stop = new NormalizedPoint(ImageWidth, ImageHeight, 1385, 759);
            leaveLine.Name = "upward driveway lane count line (leave)";

            roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            
            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            string jsonString = JsonSerializer.Serialize(roadDefinition);
            Assert.AreEqual(rdJson, jsonString);
        }

        [Test]
        public void Test_RoadDefinition_LoadMode()
        {
            RoadDefinition roadDefinition = JsonSerializer.Deserialize<RoadDefinition>(rdJson);
            Assert.NotNull(roadDefinition);
            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            Assert.AreEqual(2, roadDefinition.AnalysisAreas.Count);
            Assert.AreEqual(1, roadDefinition.ExcludedAreas.Count);
            Assert.AreEqual(5, roadDefinition.Lanes.Count);
            Assert.AreEqual(1, roadDefinition.CountLines.Count);
        }
    }
}