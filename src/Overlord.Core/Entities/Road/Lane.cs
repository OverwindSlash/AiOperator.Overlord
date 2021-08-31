using Overlord.Core.Entities.Geometric;
using System.Collections.Generic;

namespace Overlord.Core.Entities.Road
{
    public enum LaneType
    {
        DriveLane = 0,
        EmergencyLane = 1,
        RampLane = 2
    }
    
    public class Lane : NormalizedPolygon
    {
        private LaneType _type;

        public string Name { get; set; }
        public int Index { get; set; }

        public LaneType Type
        {
            get => _type;
            set
            {
                _type = value;
                SetDefaultForbiddenTypes(_type);
            }
        }

        private void SetDefaultForbiddenTypes(LaneType type)
        {
            switch (type)
            {
                case LaneType.DriveLane:
                case LaneType.RampLane:
                    this.ForbiddenTypes.Add("person");
                    this.ForbiddenTypes.Add("bicycle");
                    this.ForbiddenTypes.Add("motorbike");
                    break;
                case LaneType.EmergencyLane:
                    this.ForbiddenTypes.Add("person");
                    this.ForbiddenTypes.Add("bicycle");
                    this.ForbiddenTypes.Add("motorbike");
                    this.ForbiddenTypes.Add("car");
                    this.ForbiddenTypes.Add("bus");
                    this.ForbiddenTypes.Add("truck");
                    this.ForbiddenTypes.Add("train");
                    break;
                default:
                    break;
            }
        }

        public HashSet<string> ForbiddenTypes { get; set; }

        public Lane()
        {
            ForbiddenTypes = new HashSet<string>();
        }
    }
}
