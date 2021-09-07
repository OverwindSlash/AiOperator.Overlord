using Overlord.Domain.Interfaces;
using System.Threading.Tasks;

namespace Overlord.Domain.Event
{
    public class EventPublisher
    {
        private readonly ITrafficEventPublisher _publisher;

        public EventPublisher(ITrafficEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task<bool> Publish(TrafficEvent forbiddenEvent)
        {
            // TODO:
            return false;
        }
    }
}
