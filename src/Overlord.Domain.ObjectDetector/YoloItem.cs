namespace Overlord.Domain.ObjectDetector
{
    public class YoloItem
    {
        public int TypeId { get; set; }
        public string Type { get; set; }
        public float Confidence { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public uint TrackingId { get; set; }
    }
}
 