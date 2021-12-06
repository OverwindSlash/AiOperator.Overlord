using System;

namespace Overlord.Domain
{
    public class ObserverBase<T> : IObserver<T>
    {
        public void OnCompleted()
        {
            // Do nothing.
        }

        public void OnError(Exception error)
        {
            // Do nothing.
        }

        public virtual void OnNext(T value)
        {
            // Do nothing.
        }
    }
}
