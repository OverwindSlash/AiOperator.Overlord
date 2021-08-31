using Overlord.Core.Entities.Frame;
using System.Collections.Generic;

namespace Overlord.Domain.Interfaces
{
    public interface IMultiObjectTracker
    {
        void Track(List<TrafficObjectInfo> trafficObjectInfos);
    }
}
