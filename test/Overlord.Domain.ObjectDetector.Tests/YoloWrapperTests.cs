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
    [TestFixture]
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
    }
}
