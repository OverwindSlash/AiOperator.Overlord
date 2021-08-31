using System;
using System.Text.Json.Serialization;

namespace Overlord.Core.Entities.Geometric
{
    public class NormalizedPoint : ImageBasedGeometric
    {
        private int _originalX;
        private int _originalY;
        
        private double _normalizedX;
        private double _normalizedY;

        [JsonIgnore]
        public int OriginalX
        {
            get
            {
                CheckImageSizeInitialized();
                return _originalX;
            }
        }

        [JsonIgnore]
        public int OriginalY
        {
            get
            {
                CheckImageSizeInitialized();
                return _originalY;
            }
        }

        public double NormalizedX
        {
            get
            {
                CheckImageSizeInitialized();
                return _normalizedX;
            }
            set => _normalizedX = value;
        }

        public double NormalizedY
        {
            get
            {
                CheckImageSizeInitialized();
                return _normalizedY;
            }
            set => _normalizedY = value;
        }

        // For generate point by json deserialization
        public NormalizedPoint()
        {
            _normalizedX = 0;
            _normalizedY = 0;
        }

        public override void SetImageSize(int width, int height)
        {
            base.SetImageSize(width, height);

            _originalX = (int)Math.Round(width * _normalizedX);
            _originalY = (int)Math.Round(height * _normalizedY);
        }

        // For generate point by hand
        public NormalizedPoint(int imageWidth, int imageHeight, int originalX, int originalY)
            : base (imageWidth, imageHeight)
        {
            _originalX = originalX;
            _originalY = originalY;

            _normalizedX = (double)_originalX / imageWidth;
            _normalizedY = (double)_originalY / imageHeight;
        }
    }
}
