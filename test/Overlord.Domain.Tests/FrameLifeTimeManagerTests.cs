using NSubstitute;
using NUnit.Framework;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Domain.Event;
using Overlord.Domain.Pipeline;
using System;
using System.Collections.Generic;

namespace Overlord.Domain.Tests
{
    [TestFixture]
    public class FrameLifeTimeManagerTests
    {
        [Test]
        public void TestGenerateFrameLifeTimeManager()
        {
            FrameLifeTimeManager frameLtManager = new FrameLifeTimeManager(3);
            Assert.AreEqual(3, frameLtManager.GetFrameCount());
            Assert.AreEqual(0, frameLtManager.GetCurrentIndex());
        }

        [Test]
        public void TestAddFrameInfo_NotCircledYet_WithOutObjectInfos()
        {
            FrameLifeTimeManager frameLtManager = new FrameLifeTimeManager(3);

            FrameInfo fi1 = new FrameInfo(1L, new Mat());
            FrameInfo fi2 = new FrameInfo(2L, new Mat());
            FrameInfo fi3 = new FrameInfo(3L, new Mat());
            
            Assert.AreEqual(3, frameLtManager.GetFrameCount());

            Assert.AreEqual(0, frameLtManager.GetCurrentIndex());
            frameLtManager.AddFrameInfo(fi1);
            Assert.AreEqual(1, frameLtManager.GetCurrentIndex());
            Assert.AreEqual(1L, frameLtManager.GetFrameInfoByIndex(0).FrameId);

            Assert.AreEqual(1, frameLtManager.GetCurrentIndex());
            frameLtManager.AddFrameInfo(fi2);
            Assert.AreEqual(2, frameLtManager.GetCurrentIndex());
            Assert.AreEqual(2L, frameLtManager.GetFrameInfoByIndex(1).FrameId);

            Assert.AreEqual(2, frameLtManager.GetCurrentIndex());
            frameLtManager.AddFrameInfo(fi3);
            Assert.AreEqual(0, frameLtManager.GetCurrentIndex());
            Assert.AreEqual(3L, frameLtManager.GetFrameInfoByIndex(2).FrameId);
        }

        [Test]
        public void TestAddFrameInfo_NotCircledYet_WithObjectInfos()
        {
            FrameLifeTimeManager frameLtManager = new FrameLifeTimeManager(3);

            // car:1
            FrameInfo fi1 = new FrameInfo(1L, new Mat());
            List<TrafficObjectInfo> tois1 = new List<TrafficObjectInfo>();
            tois1.Add(new TrafficObjectInfo() {Type = "car", TrackingId = 1});
            fi1.TrafficObjectInfos = tois1;

            // car:1, person:1
            FrameInfo fi2 = new FrameInfo(2L, new Mat());
            List<TrafficObjectInfo> tois2 = new List<TrafficObjectInfo>();
            tois2.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            tois2.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 1 });
            fi2.TrafficObjectInfos = tois2;

            // person:2, truck:1
            FrameInfo fi3 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois3 = new List<TrafficObjectInfo>();
            tois3.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois3.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi3.TrafficObjectInfos = tois3;

            frameLtManager.AddFrameInfo(fi1);
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("truck:1"));

            frameLtManager.AddFrameInfo(fi2);
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("truck:1"));

            frameLtManager.AddFrameInfo(fi3);
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("truck:1"));
        }

        [Test]
        public void TestAddFrameInfo_WithCircle_WithObjectInfos()
        {
            FrameLifeTimeManager frameLtManager = new FrameLifeTimeManager(3);

            // car:1
            Mat fi1Scene = Substitute.For<Mat>();
            FrameInfo fi1 = Substitute.For<FrameInfo>(1L, fi1Scene);
            List<TrafficObjectInfo> tois1 = new List<TrafficObjectInfo>();
            tois1.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            fi1.TrafficObjectInfos = tois1;

            // car:1, person:1
            Mat fi2Scene = Substitute.For<Mat>();
            FrameInfo fi2 = Substitute.For<FrameInfo>(2L, fi2Scene);
            List<TrafficObjectInfo> tois2 = new List<TrafficObjectInfo>();
            tois2.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            tois2.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 1 });
            fi2.TrafficObjectInfos = tois2;

            // person:2, truck:1
            FrameInfo fi3 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois3 = new List<TrafficObjectInfo>();
            tois3.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois3.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi3.TrafficObjectInfos = tois3;

            // person:2, truck1
            FrameInfo fi4 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois4 = new List<TrafficObjectInfo>();
            tois4.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois4.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi4.TrafficObjectInfos = tois4;

            // truck:1
            FrameInfo fi5 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois5 = new List<TrafficObjectInfo>();
            tois5.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi5.TrafficObjectInfos = tois5;

            frameLtManager.AddFrameInfo(fi1);
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("truck:1"));

            frameLtManager.AddFrameInfo(fi2);
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("truck:1"));

            frameLtManager.AddFrameInfo(fi3);
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("truck:1"));

            frameLtManager.AddFrameInfo(fi4);
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(1, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("truck:1"));
            fi1.Received().Dispose();
            fi1Scene.Received().Dispose();

            frameLtManager.AddFrameInfo(fi5);
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("car:1"));
            Assert.AreEqual(0, frameLtManager.GetExistenceCountById("person:1"));
            Assert.AreEqual(2, frameLtManager.GetExistenceCountById("person:2"));
            Assert.AreEqual(3, frameLtManager.GetExistenceCountById("truck:1"));
            fi2.Received().Dispose();
            fi2Scene.Received().Dispose();
        }

        [Test]
        public void TestSubcribeWithObjectExpiredEventHandler()
        {
            FrameLifeTimeManager frameLtManager = new FrameLifeTimeManager(3);

            IObserver<ObjectExpiredEvent> objExpHandler = Substitute.For<IObserver<ObjectExpiredEvent>>();

            frameLtManager.Subscribe(objExpHandler);

            // car:1
            FrameInfo fi1 = new FrameInfo(1L, new Mat());
            List<TrafficObjectInfo> tois1 = new List<TrafficObjectInfo>();
            tois1.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            fi1.TrafficObjectInfos = tois1;

            // car:1, person:1
            FrameInfo fi2 = new FrameInfo(2L, new Mat());
            List<TrafficObjectInfo> tois2 = new List<TrafficObjectInfo>();
            tois2.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            tois2.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 1 });
            fi2.TrafficObjectInfos = tois2;

            // person:2, truck:1
            FrameInfo fi3 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois3 = new List<TrafficObjectInfo>();
            tois3.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois3.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi3.TrafficObjectInfos = tois3;

            // person:2, truck:1
            FrameInfo fi4 = new FrameInfo(4L, new Mat());
            List<TrafficObjectInfo> tois4 = new List<TrafficObjectInfo>();
            tois4.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois4.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi4.TrafficObjectInfos = tois4;
            
            // truck:1
            FrameInfo fi5 = new FrameInfo(5L, new Mat());
            List<TrafficObjectInfo> tois5 = new List<TrafficObjectInfo>();
            tois5.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi5.TrafficObjectInfos = tois5;

            // car:1 in FrameInfo(1L) and FrameInfo(2L), so will not be disposed.
            frameLtManager.AddFrameInfo(fi1);
            frameLtManager.AddFrameInfo(fi2);
            frameLtManager.AddFrameInfo(fi3);
            frameLtManager.AddFrameInfo(fi4);
            objExpHandler.DidNotReceive().OnNext(Arg.Any<ObjectExpiredEvent>());

            // car:1 not occured after FrameInfo(2L), so will be disposed.
            frameLtManager.AddFrameInfo(fi5);
            objExpHandler.Received().OnNext(Arg.Any<ObjectExpiredEvent>());
        }

        [Test]
        public void TestSubcribeWithFrameExpiredEventHandler()
        {
            FrameLifeTimeManager frameLtManager = new FrameLifeTimeManager(3);

            IObserver<FrameExpiredEvent> frmExpHandler = Substitute.For<IObserver<FrameExpiredEvent>>();

            frameLtManager.Subscribe(frmExpHandler);

            // car:1
            FrameInfo fi1 = new FrameInfo(1L, new Mat());
            List<TrafficObjectInfo> tois1 = new List<TrafficObjectInfo>();
            tois1.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            fi1.TrafficObjectInfos = tois1;

            // car:1, person:1
            FrameInfo fi2 = new FrameInfo(2L, new Mat());
            List<TrafficObjectInfo> tois2 = new List<TrafficObjectInfo>();
            tois2.Add(new TrafficObjectInfo() { Type = "car", TrackingId = 1 });
            tois2.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 1 });
            fi2.TrafficObjectInfos = tois2;

            // person:2, truck:1
            FrameInfo fi3 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois3 = new List<TrafficObjectInfo>();
            tois3.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois3.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi3.TrafficObjectInfos = tois3;
            
            // person:2, truck:1
            FrameInfo fi4 = new FrameInfo(3L, new Mat());
            List<TrafficObjectInfo> tois4 = new List<TrafficObjectInfo>();
            tois4.Add(new TrafficObjectInfo() { Type = "person", TrackingId = 2 });
            tois4.Add(new TrafficObjectInfo() { Type = "truck", TrackingId = 1 });
            fi4.TrafficObjectInfos = tois4;

            // FrameInfo object count not exceed capacity of FrameLifeTimeManager
            frameLtManager.AddFrameInfo(fi1);
            frameLtManager.AddFrameInfo(fi2);
            frameLtManager.AddFrameInfo(fi3);
            // so no FrameInfo will be disposed.
            frmExpHandler.DidNotReceive().OnNext(Arg.Any<FrameExpiredEvent>());

            // FrameInfo object count already exceed capacity of FrameLifeTimeManager
            frameLtManager.AddFrameInfo(fi4);
            // so FrameInfo will be disposed.
            frmExpHandler.Received().OnNext(Arg.Any<FrameExpiredEvent>());
        }
    }
}
