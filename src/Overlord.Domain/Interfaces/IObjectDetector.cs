using System;
using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using System.Collections.Generic;

namespace Overlord.Domain.Interfaces
{
    public interface IObjectDetector : IDisposable
    {
        public int SetTrackingParam(bool changeHistory, int frameStory, int maxDistance);
        public List<TrafficObjectInfo> Detect(Mat imageData, float thresh);
    }
}
