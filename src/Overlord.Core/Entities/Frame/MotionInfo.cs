namespace Overlord.Core.Entities.Frame
{
    public class MotionInfo
    {
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public double Offset { get; set; }
        
        public double Speed
        {
            get
            {
                return CalculateSpeed();
            }
        }

        private double CalculateSpeed()
        {
            throw new System.NotImplementedException();
        }
    }
}