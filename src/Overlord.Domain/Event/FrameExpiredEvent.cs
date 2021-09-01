using Overlord.Core.Entities.Frame;

namespace Overlord.Domain.Event
{
    public class FrameExpiredEvent : EventBase
    {
        private readonly long _frameId;

        public long FrameId => _frameId;

        public FrameExpiredEvent(long frameId)
        {
            _frameId = frameId;
        }

        public FrameExpiredEvent(FrameInfo frameInfo)
        {
            _frameId = frameInfo.FrameId;
        }
    }
}
