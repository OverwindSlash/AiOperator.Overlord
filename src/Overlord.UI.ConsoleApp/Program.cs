using CommandLine;
using OpenCvSharp;
using Overlord.Application;
using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Geometric;
using Overlord.Domain.Pipeline;
using Overlord.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Overlord.UI.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(Run);
        }

        private static void Run(CmdOptions option)
        {
            AnalysisEngineAppService engineAppService = new AnalysisEngineAppService();
            engineAppService.InitializeEngine(option.AppSettingFile);
            ApplicationSettings appSettings = engineAppService.AppSettings;

            List<AnalysisPipeline> pipelines = new List<AnalysisPipeline>();
            foreach (var pipelineSetting in appSettings.PipelineSettings)
            {
                AnalysisPipeline pipeline = engineAppService.OpenAnalysisPipeline(pipelineSetting);
                pipelines.Add(pipeline);

                // Task.Run(() =>
                // {
                //     PerformPipelineAnalysis(pipeline);
                // });
            }

            // TODO: Support multi-thread
            PerformPipelineAnalysis(pipelines[option.PipelineIndex], option.NoDisplay);
        }

        private static void PerformPipelineAnalysis(AnalysisPipeline pipeline, bool noDisplay)
        {
            VideoCapture capture = new VideoCapture(pipeline.VideoUri, VideoCaptureAPIs.FFMPEG);
            if (!capture.IsOpened())
            {
                string errorMsg = $"[ERROR] Pipeline video source not available.";
                Console.WriteLine(errorMsg);
                return;
            }

            long frameId = 0;
            bool gotFrame = true;

            while (gotFrame)
            {
                var frame = new Mat();

                gotFrame = ExtractFrame(capture, frame);
                if (gotFrame)
                {
                    frameId++;
                }
                else
                {
                    continue;
                }

                if (frameId % 7500 == 0)
                {
                    Console.WriteLine($"{DateTime.Now.ToString()} Analysising...");
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                FrameInfo frameInfo = new FrameInfo(frameId, frame);
                pipeline.Analyze(frameInfo);

                // DrawRegion(pipeline.RoadDef.AnalysisAreas[0], frame, Scalar.Red);
                // DrawRegion(pipeline.RoadDef.ExcludedAreas[0], frame, Scalar.Red);
                // DrawRegion(pipeline.RoadDef.Lanes[0], frame, Scalar.Blue);
                // DrawRegion(pipeline.RoadDef.Lanes[1], frame, Scalar.White);
                // DrawRegion(pipeline.RoadDef.Lanes[2], frame, Scalar.Yellow);
                // DrawRegion(pipeline.RoadDef.Lanes[3], frame, Scalar.Blue);

                foreach (TrafficObjectInfo toi in frameInfo.ObjectInfos)
                {
                    if (toi.IsAnalyzable)
                    {
                        // Display box for all objects.
                        //frame.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red);

                        // Display id.
                        //frame.PutText(toi.TrackingId.ToString(), new Point(toi.X, toi.Y - 20), HersheyFonts.HersheyPlain, 1.0, Scalar.White);

                        // Display speed.
                        //frame.PutText(toi.MotionInfo.Speed.ToString("F0"), new Point(toi.X+10, toi.Y), HersheyFonts.HersheyPlain, 1.0, Scalar.LightCyan);

                        string statusMsg = CheckTrafficObjectInfoStatus(toi);

                        if (!string.IsNullOrEmpty(statusMsg))
                        {
                            frame.PutText(statusMsg, new Point(toi.X, toi.Y + 20), HersheyFonts.HersheyPlain, 1.0, Scalar.Red);
                            frame.Rectangle(new Point(toi.X, toi.Y), new Point(toi.X + toi.Width, toi.Y + toi.Height), Scalar.Red);
                        }

                        PromptTrafficObjectInfoEvents(toi);

                        //g.DrawString($"{objectInfo.Id} L:{objectInfo.LaneIndex} O:{objectInfo.Offset.ToString("F1")}", new Font("Consolas", 10F), new SolidBrush(Color.White), new PointF(objectInfo.X + 10, objectInfo.Y + 10));
                        //g.DrawString($"{objectInfo.Id} {objectInfo.Offset.ToString("F0")}", new Font("Consolas", 10F), new SolidBrush(Color.White), new PointF(objectInfo.X + 10, objectInfo.Y + 10));
                    }
                }

                sw.Stop();

                if (!noDisplay)
                {
                    Cv2.ImShow(pipeline.Name, frame.Resize(new Size(1280, 720)));
                }

                int delay = 25 - (int)sw.ElapsedMilliseconds;
                if (delay < 1)
                {
                    Cv2.WaitKey(1);
                }
                else
                {
                    Cv2.WaitKey(delay);
                }

                sw.Reset();
            }
        }

        private static string CheckTrafficObjectInfoStatus(TrafficObjectInfo objectInfo)
        {
            string message = string.Empty;

            if (objectInfo.InStatusSlowSpeed)
            {
                message += "S ";
                //Console.WriteLine($"慢速事件：{objectInfo.Id}");
            }

            if (objectInfo.InStatusEnterForbiddenRegion)
            {
                message += "F ";
                //Console.WriteLine($"禁行事件：{objectInfo.Id}");
            }

            if (objectInfo.InStatusStopped)
            {
                message += "P ";
                //Console.WriteLine($"禁停事件：{objectInfo.Id}");
            }

            return message;
        }

        private static void PromptTrafficObjectInfoEvents(TrafficObjectInfo objectInfo)
        {
            if (objectInfo.EventSlowVehicleRaised)
            {
                Console.WriteLine($"车辆慢速事件：{objectInfo.Id}");
            }

            if (objectInfo.EventForbiddenTypeRaised)
            {
                Console.WriteLine($"禁行闯入事件：{objectInfo.Id}");
            }

            if (objectInfo.EventStoppedVehicleRaised)
            {
                Console.WriteLine($"车辆停驶事件：{objectInfo.Id}");
            }

            if (objectInfo.EventRoadAmbleRaised)
            {
                Console.WriteLine($"道路缓行事件：车道{objectInfo.LaneIndex}");
            }

            if (objectInfo.EventRoadJamRaised)
            {
                Console.WriteLine($"道路拥堵事件：车道{objectInfo.LaneIndex}");
            }
        }

        private static bool ExtractFrame(VideoCapture capture, Mat frame)
        {
            try
            {
                bool result = capture.Read(frame);
                if ((frame.Width == 0) || (frame.Height == 0))
                {
                    return false;
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private static void DrawRegion(NormalizedPolygon region, Mat frame, Scalar color)
        {
            List<Point> points = new List<Point>();
            foreach (NormalizedPoint normalizedPoint in region.Points)
            {
                var point = new Point(normalizedPoint.OriginalX, normalizedPoint.OriginalY);
                points.Add(point);
            }

            List<IEnumerable<Point>> allPoints = new List<IEnumerable<Point>>();
            allPoints.Add(points);

            frame.Polylines(allPoints, true, color);
        }
    }
}
