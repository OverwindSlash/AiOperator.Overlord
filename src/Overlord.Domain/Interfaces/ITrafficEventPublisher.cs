using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overlord.Domain.Event;

namespace Overlord.Domain.Interfaces
{
    public interface ITrafficEventPublisher
    {
        Task<bool> ReportEvent(TrafficEvent message);
    }
}
