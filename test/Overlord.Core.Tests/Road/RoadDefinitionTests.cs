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

        private string rdJson = "{\"RoadName\":\"hc_k41_700_1\",\"DeviceNo\":\"DAF6AD5D3A9BDE8FE310\",\"PrePositionName\":\"prepos0\",\"DetectionThresh\":0.6,\"TrackingChangeHistory\":true,\"TrackingFramesStory\":50,\"TrackingMaxDistance\":40," +
                                "\"IsObjectAnalyzableRetain\":false,\"MaxObjectSnapshots\":10,\"MotionCalculationFrameInterval\":10," +
                                "\"UvQuadrilateral\":[1326,598,901,620,1025,299,1131,276],\"LonLatQuadrilateral\":[120.658646,31.224102,120.65872,31.224012,120.65726,31.223541,120.656845,31.223684]," +
                                "\"IsDoubleLineCounting\":false,\"DriveLaneForbiddenDurationFrame\":10,\"EmergencyLaneForbiddenDurationFrame\":15,\"StopEventSpeedUpperLimit\":10,\"StopEventEnableDurationSec\":5," +
                                "\"SlowVehicleSpeedUpperLimit\":30,\"SlowVehicleSpeedLowerLimit\":10,\"SlowVehicleEnableDurationSec\":5,\"MinSlowEventsToJudgeAmble\":5,\"AmbleJudgeDurationSec\":30,\"MinStopEventsToJudgeJam\":5,\"JamJudgeDurationSec\":30," + 
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
            roadDefinition.DeviceNo = "DAF6AD5D3A9BDE8FE310";
            roadDefinition.PrePositionName = "prepos0";

            // detection param
            roadDefinition.DetectionThresh = 0.6F;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            // analysis step param
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.MotionCalculationFrameInterval = 10;

            // pixel to geography coordinates.
            int[] uvs = { 1326, 598, 901, 620, 1025, 299, 1131, 276 };
            roadDefinition.UvQuadrilateral.AddRange(uvs);

            float[] lonlats = { (float)120.6586438007756, (float)31.224101211225364, (float)120.65872464827149, (float)31.22401242993521,
                (float)120.65726041029022, (float)31.223541501683588, (float)120.65684269822808, (float)31.223684324436487};
            roadDefinition.LonLatQuadrilateral.AddRange(lonlats);

            // double line counting
            roadDefinition.IsDoubleLineCounting = false;

            // event detection parameters
            roadDefinition.DriveLaneForbiddenDurationFrame = 10;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 15;
            roadDefinition.StopEventSpeedUpperLimit = 10;
            roadDefinition.StopEventEnableDurationSec = 5;
            roadDefinition.SlowVehicleSpeedUpperLimit = 30;
            roadDefinition.SlowVehicleSpeedLowerLimit = 10;
            roadDefinition.SlowVehicleEnableDurationSec = 5;
            roadDefinition.MinSlowEventsToJudgeAmble = 5;
            roadDefinition.AmbleJudgeDurationSec = 30;
            roadDefinition.MinStopEventsToJudgeJam = 5;
            roadDefinition.JamJudgeDurationSec = 30;


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

        [Test]
        public void TestSaveRoadDefinition_hc_k41_700_2()
        {
            int w = 1920;
            int h = 1080;

            RoadDefinition roadDefinition = new RoadDefinition();

            roadDefinition.RoadName = "hc_k41_700_2";
            roadDefinition.DeviceNo = "DAF6AD5D3A9BDE8FE310";
            roadDefinition.PrePositionName = "prepos0";
            
            // analysis param
            roadDefinition.DetectionThresh = 0.6F;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            // analysis step param
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.MotionCalculationFrameInterval = 5;

            // pixel to geography coordinates.
            int[] uvs = { 1327, 599, 1312, 276, 1026, 299, 900, 621 };
            roadDefinition.UvQuadrilateral.AddRange(uvs);

            float[] lonlats = { (float)120.6586424915274, (float)31.22410363108796, (float)120.65684588050745, (float)31.223679024166824,
                (float)120.65726808409714, (float)31.22353620140584, (float)120.6587233390233, (float)31.22400712968413};
            roadDefinition.LonLatQuadrilateral.AddRange(lonlats);

            // double line counting
            roadDefinition.IsDoubleLineCounting = true;

            // event detection parameters
            roadDefinition.DriveLaneForbiddenDurationFrame = 10;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 15;
            roadDefinition.StopEventSpeedUpperLimit = 10;
            roadDefinition.StopEventEnableDurationSec = 5;
            roadDefinition.SlowVehicleSpeedUpperLimit = 30;
            roadDefinition.SlowVehicleSpeedLowerLimit = 10;
            roadDefinition.SlowVehicleEnableDurationSec = 5;
            roadDefinition.MinSlowEventsToJudgeAmble = 5;
            roadDefinition.AmbleJudgeDurationSec = 30;
            roadDefinition.MinStopEventsToJudgeJam = 5;
            roadDefinition.JamJudgeDurationSec = 30;

            {
                // tracking area
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 707));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 982, 296));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1444, 292));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1432, 1080));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 1080));
                analysisArea.Name = "driveway region";

                roadDefinition.AddAnalysisArea(analysisArea);
            }
            
            {
                // lane 1
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 707));
                lane.AddPoint(new NormalizedPoint(w, h, 982, 296));
                lane.AddPoint(new NormalizedPoint(w, h, 1024, 296));
                lane.AddPoint(new NormalizedPoint(w, h, 957, 338));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 790));
                lane.Name = "downward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 1;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 2
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 790));
                lane.AddPoint(new NormalizedPoint(w, h, 957, 338));
                lane.AddPoint(new NormalizedPoint(w, h, 1024, 296));
                lane.AddPoint(new NormalizedPoint(w, h, 1171, 294));
                lane.AddPoint(new NormalizedPoint(w, h, 72, 1080));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 1080));
                lane.Name = "downward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 2;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 3
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 72, 1080));
                lane.AddPoint(new NormalizedPoint(w, h, 1171, 294));
                lane.AddPoint(new NormalizedPoint(w, h, 1326, 292));
                lane.AddPoint(new NormalizedPoint(w, h, 1341, 329));
                lane.AddPoint(new NormalizedPoint(w, h, 1350, 387));
                lane.AddPoint(new NormalizedPoint(w, h, 1339, 546));
                lane.AddPoint(new NormalizedPoint(w, h, 1209, 1080));
                lane.Name = "upward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 3;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 4
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 1209, 1080));
                lane.AddPoint(new NormalizedPoint(w, h, 1339, 546));
                lane.AddPoint(new NormalizedPoint(w, h, 1350, 387));
                lane.AddPoint(new NormalizedPoint(w, h, 1341, 329));
                lane.AddPoint(new NormalizedPoint(w, h, 1326, 292));
                lane.AddPoint(new NormalizedPoint(w, h, 1443, 292));
                lane.AddPoint(new NormalizedPoint(w, h, 1432, 1080));
                lane.Name = "upward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 4;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // count line (upward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 825, 623);
                enterLine.Stop = new NormalizedPoint(w, h, 1437, 623);
                enterLine.Name = "upward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 1024, 457);
                leaveLine.Stop = new NormalizedPoint(w, h, 1347, 457);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }

            {
                // count line (downward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 555, 485);
                enterLine.Stop = new NormalizedPoint(w, h, 992, 485);
                enterLine.Name = "downward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 174, 644);
                leaveLine.Stop = new NormalizedPoint(w, h, 790, 644);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }


            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            string jsonString = JsonSerializer.Serialize(roadDefinition);
        }

        [Test]
        public void TestSaveRoadDefinition_hc_k41_700_1()
        {
            int w = 1920;
            int h = 1080;

            RoadDefinition roadDefinition = new RoadDefinition();

            roadDefinition.RoadName = "hc_k41_700_1";
            roadDefinition.DeviceNo = "A03A7F963117B78E2BF6";
            roadDefinition.PrePositionName = "prepos0";

            // analysis param
            roadDefinition.DetectionThresh = 0.6F;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            // analysis step param
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.MotionCalculationFrameInterval = 10;

            // pixel to geography coordinates.
            int[] uvs = { 1379, 809, 989, 270, 770, 248, 796, 770 };
            roadDefinition.UvQuadrilateral.AddRange(uvs);

            float[] lonlats = { (float)120.6597390100557, (float)31.22418422937701, (float)120.66195333313779, (float)31.224269150456937,
                (float)120.66243392658563, (float)31.22454707345248, (float)120.65975248463835, (float)31.224292310738083};
            roadDefinition.LonLatQuadrilateral.AddRange(lonlats);

            // double line counting
            roadDefinition.IsDoubleLineCounting = false;

            // event detection parameters
            roadDefinition.DriveLaneForbiddenDurationFrame = 25;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 25;
            roadDefinition.StopEventSpeedUpperLimit = 10;
            roadDefinition.StopEventEnableDurationSec = 5;
            roadDefinition.SlowVehicleSpeedUpperLimit = 30;
            roadDefinition.SlowVehicleSpeedLowerLimit = 10;
            roadDefinition.SlowVehicleEnableDurationSec = 5;
            roadDefinition.MinSlowEventsToJudgeAmble = 5;
            roadDefinition.AmbleJudgeDurationSec = 30;
            roadDefinition.MinStopEventsToJudgeJam = 5;
            roadDefinition.JamJudgeDurationSec = 30;

            {
                // tracking area
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(w, h, 583, 1080));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 590, 540));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 664, 279));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1034, 280));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1920, 648));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1920, 1080));
                analysisArea.Name = "driveway region";

                roadDefinition.AddAnalysisArea(analysisArea);
            }

            {
                // lane 1
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 663, 1080));
                lane.AddPoint(new NormalizedPoint(w, h, 631, 578));
                lane.AddPoint(new NormalizedPoint(w, h, 656, 365));
                lane.AddPoint(new NormalizedPoint(w, h, 698, 278));
                lane.AddPoint(new NormalizedPoint(w, h, 738, 278));
                lane.AddPoint(new NormalizedPoint(w, h, 724, 305));
                lane.AddPoint(new NormalizedPoint(w, h, 717, 351));
                lane.AddPoint(new NormalizedPoint(w, h, 732, 505));
                lane.AddPoint(new NormalizedPoint(w, h, 768, 680));
                lane.AddPoint(new NormalizedPoint(w, h, 882, 1080));
                lane.Name = "downward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 1;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 2
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 1080, 466));
                lane.AddPoint(new NormalizedPoint(w, h, 932, 344));
                lane.AddPoint(new NormalizedPoint(w, h, 881, 280));
                lane.AddPoint(new NormalizedPoint(w, h, 738, 277));
                lane.AddPoint(new NormalizedPoint(w, h, 723, 306));
                lane.AddPoint(new NormalizedPoint(w, h, 717, 349));
                lane.AddPoint(new NormalizedPoint(w, h, 731, 503));
                lane.AddPoint(new NormalizedPoint(w, h, 766, 680));
                lane.AddPoint(new NormalizedPoint(w, h, 882, 1077));
                lane.AddPoint(new NormalizedPoint(w, h, 1920, 1080));
                lane.Name = "downward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 2;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 3
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 1920, 722));
                lane.AddPoint(new NormalizedPoint(w, h, 1242, 408));
                lane.AddPoint(new NormalizedPoint(w, h, 1066, 321));
                lane.AddPoint(new NormalizedPoint(w, h, 1023, 296));
                lane.AddPoint(new NormalizedPoint(w, h, 1001, 280));
                lane.AddPoint(new NormalizedPoint(w, h, 882, 279));
                lane.AddPoint(new NormalizedPoint(w, h, 933, 344));
                lane.AddPoint(new NormalizedPoint(w, h, 1080, 465));
                lane.AddPoint(new NormalizedPoint(w, h, 1920, 1080));

                lane.Name = "upward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 3;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 4
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 1920, 676));
                lane.AddPoint(new NormalizedPoint(w, h, 1202, 370));
                lane.AddPoint(new NormalizedPoint(w, h, 1077, 309));
                lane.AddPoint(new NormalizedPoint(w, h, 1028, 280));
                lane.AddPoint(new NormalizedPoint(w, h, 1001, 280));
                lane.AddPoint(new NormalizedPoint(w, h, 1022, 295));
                lane.AddPoint(new NormalizedPoint(w, h, 1065, 320));
                lane.AddPoint(new NormalizedPoint(w, h, 1242, 408));
                lane.AddPoint(new NormalizedPoint(w, h, 1920, 723));
                lane.Name = "upward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 4;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // count line (upward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 1326, 647);
                enterLine.Stop = new NormalizedPoint(w, h, 1792, 622);
                enterLine.Name = "upward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 1340, 428);
                leaveLine.Stop = new NormalizedPoint(w, h, 1035, 430);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }

            {
                // count line (downward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 1011, 427);
                enterLine.Stop = new NormalizedPoint(w, h, 652, 426);
                enterLine.Name = "downward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 642, 643);
                leaveLine.Stop = new NormalizedPoint(w, h, 1258, 633);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }


            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            string jsonString = JsonSerializer.Serialize(roadDefinition);
        }

        [Test]
        public void TestSaveRoadDefinition_cj_k32_500()
        {
            int w = 1280;
            int h = 720;

            RoadDefinition roadDefinition = new RoadDefinition();

            roadDefinition.RoadName = "cj_k32_500";
            roadDefinition.DeviceNo = "D07FCE52E186F0F18976";
            roadDefinition.PrePositionName = "prepos0";

            // analysis param
            roadDefinition.DetectionThresh = 0.6F;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            // analysis step param
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.MotionCalculationFrameInterval = 10;

            // pixel to geography coordinates.
            int[] uvs = { 734, 581, 1105, 357, 889, 309, 771, 348 };
            roadDefinition.UvQuadrilateral.AddRange(uvs);

            float[] lonlats = { (float)120.88446608163247, (float)31.350831136380823, (float)120.88423252219987, (float)31.350071734159588,
                (float)120.88444811552226, (float)31.349323890387456, (float)120.88456489523857, (float)31.350064024460618};
            roadDefinition.LonLatQuadrilateral.AddRange(lonlats);

            // double line counting
            roadDefinition.IsDoubleLineCounting = false;

            // event detection parameters
            roadDefinition.DriveLaneForbiddenDurationFrame = 25;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 25;
            roadDefinition.StopEventSpeedUpperLimit = 10;
            roadDefinition.StopEventEnableDurationSec = 5;
            roadDefinition.SlowVehicleSpeedUpperLimit = 30;
            roadDefinition.SlowVehicleSpeedLowerLimit = 10;
            roadDefinition.SlowVehicleEnableDurationSec = 5;
            roadDefinition.MinSlowEventsToJudgeAmble = 5;
            roadDefinition.AmbleJudgeDurationSec = 30;
            roadDefinition.MinStopEventsToJudgeJam = 5;
            roadDefinition.JamJudgeDurationSec = 30;

            {
                // tracking area
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 452));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 890, 303));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1155, 301));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1123, 453));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1100, 719));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 719));
                analysisArea.Name = "driveway region";

                roadDefinition.AddAnalysisArea(analysisArea);
            }

            {
                // excluded area
                ExcludedArea excludedArea = new ExcludedArea();
                excludedArea.AddPoint(new NormalizedPoint(w, h, 1081, 411));
                excludedArea.AddPoint(new NormalizedPoint(w, h, 1133, 411));
                excludedArea.AddPoint(new NormalizedPoint(w, h, 1131, 464));
                excludedArea.AddPoint(new NormalizedPoint(w, h, 1081, 464));
                excludedArea.Name = "warn sign region";

                roadDefinition.AddExcludedArea(excludedArea);
            }

            {
                // lane 1
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 474));
                lane.AddPoint(new NormalizedPoint(w, h, 739, 341));
                lane.AddPoint(new NormalizedPoint(w, h, 911, 303));
                lane.AddPoint(new NormalizedPoint(w, h, 928, 303));
                lane.AddPoint(new NormalizedPoint(w, h, 741, 346));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 488));
                lane.Name = "downward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 1;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 2
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 718));
                lane.AddPoint(new NormalizedPoint(w, h, 1021, 303));
                lane.AddPoint(new NormalizedPoint(w, h, 930, 303));
                lane.AddPoint(new NormalizedPoint(w, h, 740, 345));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 488));
                lane.Name = "downward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 2;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 3
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 972, 719));
                lane.AddPoint(new NormalizedPoint(w, h, 1089, 395));
                lane.AddPoint(new NormalizedPoint(w, h, 1106, 357));
                lane.AddPoint(new NormalizedPoint(w, h, 1127, 319));
                lane.AddPoint(new NormalizedPoint(w, h, 1142, 302));
                lane.AddPoint(new NormalizedPoint(w, h, 1021, 302));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 719));
                lane.Name = "upward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 3;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 4
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 972, 718));
                lane.AddPoint(new NormalizedPoint(w, h, 1089, 395));
                lane.AddPoint(new NormalizedPoint(w, h, 1105, 356));
                lane.AddPoint(new NormalizedPoint(w, h, 1126, 319));
                lane.AddPoint(new NormalizedPoint(w, h, 1142, 302));
                lane.AddPoint(new NormalizedPoint(w, h, 1155, 302));
                lane.AddPoint(new NormalizedPoint(w, h, 1122, 453));
                lane.AddPoint(new NormalizedPoint(w, h, 1100, 719));
                lane.Name = "upward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 4;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // count line (upward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 680, 470);
                enterLine.Stop = new NormalizedPoint(w, h, 1115, 520);
                enterLine.Name = "upward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 1125, 415);
                leaveLine.Stop = new NormalizedPoint(w, h, 839, 394);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }

            {
                // count line (downward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 773, 402);
                enterLine.Stop = new NormalizedPoint(w, h, 527, 379);
                enterLine.Name = "downward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 326, 415);
                leaveLine.Stop = new NormalizedPoint(w, h, 656, 451);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }


            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            string jsonString = JsonSerializer.Serialize(roadDefinition);
        }

        [Test]
        public void TestSaveRoadDefinition_hc_k34_700_gx()
        {
            int w = 1280;
            int h = 720;

            RoadDefinition roadDefinition = new RoadDefinition();

            roadDefinition.RoadName = "hc_k34_700_gx";
            roadDefinition.DeviceNo = "E1B80B6A1CBC55654193";
            roadDefinition.PrePositionName = "prepos0";

            // analysis param
            roadDefinition.DetectionThresh = 0.6F;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            // analysis step param
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.MotionCalculationFrameInterval = 10;

            // pixel to geography coordinates.
            int[] uvs = { 1041, 368, 397, 713, 209, 512, 863, 331 };
            roadDefinition.UvQuadrilateral.AddRange(uvs);

            float[] lonlats = { (float)120.73062115561765, (float)31.22499762673786, (float)120.73144759668683, (float)31.225094127120364,
                (float)120.73135776613583, (float)31.224904986277053, (float)120.73036963007486, (float)31.224742865249958};
            roadDefinition.LonLatQuadrilateral.AddRange(lonlats);

            // double line counting
            roadDefinition.IsDoubleLineCounting = false;

            // event detection parameters
            roadDefinition.DriveLaneForbiddenDurationFrame = 25;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 25;
            roadDefinition.StopEventSpeedUpperLimit = 10;
            roadDefinition.StopEventEnableDurationSec = 5;
            roadDefinition.SlowVehicleSpeedUpperLimit = 30;
            roadDefinition.SlowVehicleSpeedLowerLimit = 10;
            roadDefinition.SlowVehicleEnableDurationSec = 5;
            roadDefinition.MinSlowEventsToJudgeAmble = 5;
            roadDefinition.AmbleJudgeDurationSec = 30;
            roadDefinition.MinStopEventsToJudgeJam = 5;
            roadDefinition.JamJudgeDurationSec = 30;

            {
                // tracking area
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 496));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 914, 291));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1142, 303));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1247, 308));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1181, 719));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 719));
                analysisArea.Name = "driveway region";

                roadDefinition.AddAnalysisArea(analysisArea);
            }

            {
                // lane 1
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 538));
                lane.AddPoint(new NormalizedPoint(w, h, 960, 293));
                lane.AddPoint(new NormalizedPoint(w, h, 971, 294));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 554));
                lane.Name = "downward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 1;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 2
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 554));
                lane.AddPoint(new NormalizedPoint(w, h, 971, 295));
                lane.AddPoint(new NormalizedPoint(w, h, 1013, 297));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 684));
                lane.Name = "downward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 2;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 3
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 684));
                lane.AddPoint(new NormalizedPoint(w, h, 1013, 297));
                lane.AddPoint(new NormalizedPoint(w, h, 1116, 302));
                lane.AddPoint(new NormalizedPoint(w, h, 657, 719));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 719));
                lane.Name = "upward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 3;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 4
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 656, 718));
                lane.AddPoint(new NormalizedPoint(w, h, 1116, 302));
                lane.AddPoint(new NormalizedPoint(w, h, 1143, 303));
                lane.AddPoint(new NormalizedPoint(w, h, 1069, 372));
                lane.AddPoint(new NormalizedPoint(w, h, 1061, 394));
                lane.AddPoint(new NormalizedPoint(w, h, 1071, 393));
                lane.AddPoint(new NormalizedPoint(w, h, 1107, 383));
                lane.AddPoint(new NormalizedPoint(w, h, 995, 466));
                lane.AddPoint(new NormalizedPoint(w, h, 705, 718));
                lane.Name = "separation region";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 4;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 5
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 704, 719));
                lane.AddPoint(new NormalizedPoint(w, h, 995, 466));
                lane.AddPoint(new NormalizedPoint(w, h, 1108, 384));
                lane.AddPoint(new NormalizedPoint(w, h, 1163, 357));
                lane.AddPoint(new NormalizedPoint(w, h, 1217, 337));
                lane.AddPoint(new NormalizedPoint(w, h, 1207, 363));
                lane.AddPoint(new NormalizedPoint(w, h, 1184, 376));
                lane.AddPoint(new NormalizedPoint(w, h, 1159, 398));
                lane.AddPoint(new NormalizedPoint(w, h, 1125, 456));
                lane.AddPoint(new NormalizedPoint(w, h, 982, 718));
                lane.Name = "upward ramp lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 5;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 5
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 982, 719));
                lane.AddPoint(new NormalizedPoint(w, h, 1125, 455));
                lane.AddPoint(new NormalizedPoint(w, h, 1158, 398));
                lane.AddPoint(new NormalizedPoint(w, h, 1185, 375));
                lane.AddPoint(new NormalizedPoint(w, h, 1207, 363));
                lane.AddPoint(new NormalizedPoint(w, h, 1135, 719));
                lane.Name = "upward ramp emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 6;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // count line (upward drive lane)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 346, 552);
                enterLine.Stop = new NormalizedPoint(w, h, 803, 633);
                enterLine.Name = "upward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 974, 485);
                leaveLine.Stop = new NormalizedPoint(w, h, 637, 440);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }

            {
                // count line (upward ramp lane)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 803, 633);
                enterLine.Stop = new NormalizedPoint(w, h, 1141, 690);
                enterLine.Name = "upward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 1177, 509);
                leaveLine.Stop = new NormalizedPoint(w, h, 974, 485);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }

            {
                // count line (downward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 771, 389);
                enterLine.Stop = new NormalizedPoint(w, h, 685, 363);
                enterLine.Name = "downward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 408, 434);
                leaveLine.Stop = new NormalizedPoint(w, h, 541, 477);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }


            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            string jsonString = JsonSerializer.Serialize(roadDefinition);
        }

        [Test]
        public void TestSaveRoadDefinition_hc_k29_100()
        {
            int w = 1280;
            int h = 720;

            RoadDefinition roadDefinition = new RoadDefinition();

            roadDefinition.RoadName = "hc_k29_100";
            roadDefinition.DeviceNo = "A1344D0C243CDF639F68";
            roadDefinition.PrePositionName = "prepos0";

            // analysis param
            roadDefinition.DetectionThresh = 0.6F;
            roadDefinition.TrackingChangeHistory = true;
            roadDefinition.TrackingFramesStory = 50;
            roadDefinition.TrackingMaxDistance = 40;

            // analysis step param
            roadDefinition.IsObjectAnalyzableRetain = false;
            roadDefinition.MaxObjectSnapshots = 10;
            roadDefinition.MotionCalculationFrameInterval = 10;

            // pixel to geography coordinates.
            int[] uvs = { 886, 492, 1024, 217, 553, 206, 283, 455 };
            roadDefinition.UvQuadrilateral.AddRange(uvs);

            float[] lonlats = { (float)120.78665764542657, (float)31.242197930852157, (float)120.78623095030933, (float)31.241804280640686,
                (float)120.78653637418273, (float)31.241642188895543, (float)120.78679688278062, (float)31.242128463288186};
            roadDefinition.LonLatQuadrilateral.AddRange(lonlats);

            // double line counting
            roadDefinition.IsDoubleLineCounting = false;

            // event detection parameters
            roadDefinition.DriveLaneForbiddenDurationFrame = 25;
            roadDefinition.EmergencyLaneForbiddenDurationFrame = 25;
            roadDefinition.StopEventSpeedUpperLimit = 10;
            roadDefinition.StopEventEnableDurationSec = 5;
            roadDefinition.SlowVehicleSpeedUpperLimit = 30;
            roadDefinition.SlowVehicleSpeedLowerLimit = 10;
            roadDefinition.SlowVehicleEnableDurationSec = 5;
            roadDefinition.MinSlowEventsToJudgeAmble = 5;
            roadDefinition.AmbleJudgeDurationSec = 30;
            roadDefinition.MinStopEventsToJudgeJam = 5;
            roadDefinition.JamJudgeDurationSec = 30;

            {
                // tracking area
                AnalysisArea analysisArea = new AnalysisArea();
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 296));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 798, 141));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1082, 139));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 1085, 256));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 986, 719));
                analysisArea.AddPoint(new NormalizedPoint(w, h, 0, 719));
                analysisArea.Name = "driveway region";

                roadDefinition.AddAnalysisArea(analysisArea);
            }

            {
                // lane 1
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 321));
                lane.AddPoint(new NormalizedPoint(w, h, 543, 201));
                lane.AddPoint(new NormalizedPoint(w, h, 573, 202));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 337));
                lane.Name = "downward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 1;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 2
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 337));
                lane.AddPoint(new NormalizedPoint(w, h, 842, 141));
                lane.AddPoint(new NormalizedPoint(w, h, 919, 141));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 491));
                lane.Name = "downward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 2;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 3
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 0, 490));
                lane.AddPoint(new NormalizedPoint(w, h, 920, 140));
                lane.AddPoint(new NormalizedPoint(w, h, 1066, 140));
                lane.AddPoint(new NormalizedPoint(w, h, 1033, 165));
                lane.AddPoint(new NormalizedPoint(w, h, 1024, 218));
                lane.AddPoint(new NormalizedPoint(w, h, 1012, 255));
                lane.AddPoint(new NormalizedPoint(w, h, 765, 719));
                lane.AddPoint(new NormalizedPoint(w, h, 0, 719));
                lane.Name = "upward driveway lane";
                lane.Type = LaneType.DriveLane;
                lane.Index = 3;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // lane 4
                Lane lane = new Lane();
                lane.AddPoint(new NormalizedPoint(w, h, 765, 719));
                lane.AddPoint(new NormalizedPoint(w, h, 1011, 218));
                lane.AddPoint(new NormalizedPoint(w, h, 1032, 165));
                lane.AddPoint(new NormalizedPoint(w, h, 1066, 140));
                lane.AddPoint(new NormalizedPoint(w, h, 1082, 139));
                lane.AddPoint(new NormalizedPoint(w, h, 1084, 256));
                lane.AddPoint(new NormalizedPoint(w, h, 985, 719));
                lane.Name = "upward emergency lane";
                lane.Type = LaneType.EmergencyLane;
                lane.Index = 4;

                roadDefinition.Lanes.Add(lane);
            }

            {
                // count line (upward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 124, 443);
                enterLine.Stop = new NormalizedPoint(w, h, 1032, 502);
                enterLine.Name = "upward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 1083, 218);
                leaveLine.Stop = new NormalizedPoint(w, h, 748, 206);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }

            {
                // count line (downward)
                EnterLine enterLine = new EnterLine();
                enterLine.Start = new NormalizedPoint(w, h, 749, 206);
                enterLine.Stop = new NormalizedPoint(w, h, 497, 200);
                enterLine.Name = "downward driveway lane count line (enter)";

                LeaveLine leaveLine = new LeaveLine();
                leaveLine.Start = new NormalizedPoint(w, h, 206, 256);
                leaveLine.Stop = new NormalizedPoint(w, h, 468, 312);
                leaveLine.Name = "upward driveway lane count line (leave)";

                roadDefinition.CountLines.Add(new Tuple<EnterLine, LeaveLine>(enterLine, leaveLine));
            }


            roadDefinition.SetImageSize(ImageWidth, ImageHeight);

            string jsonString = JsonSerializer.Serialize(roadDefinition);
        }
    }
}