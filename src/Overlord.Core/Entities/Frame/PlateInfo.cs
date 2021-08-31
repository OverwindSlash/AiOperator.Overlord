using OpenCvSharp;
using System;

namespace Overlord.Core.Entities.Frame
{
    public class PlateInfo : IDisposable
    {
        public Mat PlateCapture { get; set; }

        public void Dispose()
        {
            PlateCapture?.Dispose();
        }
    }
}