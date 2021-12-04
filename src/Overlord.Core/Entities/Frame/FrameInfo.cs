using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace Overlord.Core.Entities.Frame
{
    public class FrameInfo : IDisposable
    {
        public long FrameId { get; }

        public DateTime TimeStamp { get; }

        public Mat Scene { get; set; }
        
        public List<TrafficObjectInfo> TrafficObjectInfos { get; set; }

        public FrameInfo(long frameId, Mat scene)
        {
            FrameId = frameId;
            TimeStamp = DateTime.Now;
            Scene = scene;
        }

        public void Dispose()
        {
            Scene?.Dispose();
            foreach (TrafficObjectInfo trafficObjectInfo in TrafficObjectInfos)
            {
                trafficObjectInfo.Dispose();
            }
        }
    }
}