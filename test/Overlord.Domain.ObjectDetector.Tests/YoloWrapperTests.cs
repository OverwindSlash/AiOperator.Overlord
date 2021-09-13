using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Domain.ObjectDetector.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Overlord.Domain.ObjectDetector.Tests
{
    [TestFixture(Ignore = "Need GPU")]
    public class YoloWrapperTests
    {
        private YoloConfiguration _config;

        public YoloWrapperTests()
        {
            string configPath = @"Model/coco";
            _config = ConfigurationLoader.Load(configPath);
        }

        [Test]
        public void TestInit()
        {
            using (YoloWrapper yolo = new YoloWrapper(_config))
            {
                Assert.AreEqual(true, yolo.CudaEnv.CudaExists);
                Assert.AreEqual(true, yolo.CudaEnv.CudnnExists);
            }
        }

        private static void ShowResultImage(List<TrafficObjectInfo> items, Mat mat)
        {
            foreach (TrafficObjectInfo item in items)
            {
                mat.PutText(item.Id, new Point(item.X, item.Y), HersheyFonts.HersheyPlain, 1.0, Scalar.Aqua);
                mat.Rectangle(new Point(item.X, item.Y), new Point(item.X + item.Width, item.Y + item.Height), Scalar.Aqua);
            }

            Window.ShowImages(mat);
        }

        [Test]
        public void TestDetectMatBytes()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Traffic_001.jpg", ImreadModes.Color);
            byte[] bytes = mat.ToBytes();

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(bytes, 0.7F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            Assert.AreEqual(14, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetectMatPtr()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Traffic_001.jpg", ImreadModes.Color);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(mat, 0.7F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            //string json = JsonSerializer.Serialize(items);

            Assert.AreEqual(14, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetect2kMatBytes()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Pedestrian2K.jpg", ImreadModes.Color);
            byte[] bytes = mat.ToBytes();

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(bytes, 0.7F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            Assert.AreEqual(11, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetect2kMatPtr()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Pedestrian2K.jpg", ImreadModes.Color);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(mat, 0.7F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            Assert.AreEqual(11, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetect4kMatBytes()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Pedestrian4K.jpg", ImreadModes.Color);
            byte[] bytes = mat.ToBytes();

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(bytes, 0.7F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            Assert.AreEqual(11, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetect4kMatPtr()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Pedestrian4K.jpg", ImreadModes.Color);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(mat, 0.7F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            Assert.AreEqual(11, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetectHighwayMatPtr()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/Traffic_002.jpg", ImreadModes.Color);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(mat, 0.6F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            //string json = JsonSerializer.Serialize(items);

            Assert.AreEqual(10, items.Count);

            //ShowResultImage(items, mat);
        }

        [Test]
        public void TestDetectHighwayForMotionMatPtr()
        {
            using YoloWrapper yolo = new YoloWrapper(_config);
            using Mat mat = new Mat("Images/pl_000001.jpg", ImreadModes.Color);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var items = yolo.Detect(mat, 0.6F).ToList();
            _stopwatch.Stop();
            Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");

            //string json = JsonSerializer.Serialize(items);

            Assert.AreEqual(8, items.Count);

            //ShowResultImage(items, mat);
        }

        // [Test]
        // public void TestDetectHighwayMatPtrForMotionCalculation()
        // {
        //     using YoloWrapper yolo = new YoloWrapper(_config);
        //
        //     DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);
        //
        //     for (int i = 1; i <= 10; i++)
        //     {
        //         string filename = $"Images/pl_0000{i:D2}.jpg";
        //         using Mat mat = new Mat(filename, ImreadModes.Color);
        //
        //         Stopwatch _stopwatch = new Stopwatch();
        //         _stopwatch.Start();
        //         var items = yolo.Detect(mat, 0.6F).ToList();
        //         _stopwatch.Stop();
        //         Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");
        //
        //         FrameInfo frameInfo = new FrameInfo(i, mat);
        //         foreach (TrafficObjectInfo toi in items)
        //         {
        //             toi.FrameId = frameInfo.FrameId;
        //             toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
        //             toi.IsAnalyzable = true;
        //         }
        //
        //         string json = JsonSerializer.Serialize(items);
        //         File.WriteAllText($"Json/pl_0000{i:D2}.json", json);
        //     }
        //
        //     //ShowResultImage(items, mat);
        // }

        // [Test]
        // public void TestDetectHighwayMatPtrForCounting()
        // {
        //     using YoloWrapper yolo = new YoloWrapper(_config);
        //
        //     DateTime timestamp = new DateTime(2021, 9, 1, 18, 0, 0);
        //
        //     for (int i = 1; i <= 30; i++)
        //     {
        //         string filename = $"Images/count_0000{i:D2}.jpg";
        //         using Mat mat = new Mat(filename, ImreadModes.Color);
        //
        //         Stopwatch _stopwatch = new Stopwatch();
        //         _stopwatch.Start();
        //         var items = yolo.Detect(mat, 0.6F).ToList();
        //         _stopwatch.Stop();
        //         Console.WriteLine($"detection elapse: {_stopwatch.ElapsedMilliseconds}ms");
        //
        //         FrameInfo frameInfo = new FrameInfo(i, mat);
        //         foreach (TrafficObjectInfo toi in items)
        //         {
        //             toi.FrameId = frameInfo.FrameId;
        //             toi.TimeStamp = timestamp.AddMilliseconds(200 * i);
        //             toi.IsAnalyzable = true;
        //         }
        //
        //         string json = JsonSerializer.Serialize(items);
        //         File.WriteAllText($"Json/count_0000{i:D2}.json", json);
        //     }
        //
        //     //ShowResultImage(items, mat);
        // }
    }
}
