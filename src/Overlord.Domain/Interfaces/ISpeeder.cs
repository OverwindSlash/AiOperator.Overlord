using Overlord.Core.Entities.Frame;
using Overlord.Core.Entities.Road;

namespace Overlord.Domain.Interfaces
{
    public interface ISpeeder
    {
        void SetRoadDefinition(RoadDefinition roadDefinition);
        double CalculateSpeed(TrafficObjectInfo toi);
    }
}
