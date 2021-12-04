using OpenCvSharp;
using System;

namespace Overlord.Core.Entities.Frame
{
    public class PlateInfo : IDisposable
    {
        public Mat PlateImage { get; set; }
        
        public bool IsPlateRecognized { get; set; }
        
        public string PlateNumber { get; set; }

        public void Dispose()
        {
            PlateImage?.Dispose();
        }
    }
}