using System.Collections.Generic;
using OpenCvSharp;

namespace Overlord.Core.Entities.Camera
{
    public class CameraDefinition
    {
        public string CameraNo { get; set; }
        public string CameraType { get; set; }
        public Size VideoSize { get; set; }
        public float Latitude { get; set; }
        public float Longtitude { get; set; }
        public List<PreserPosition> PreserPositions { get; set; }
    }
}