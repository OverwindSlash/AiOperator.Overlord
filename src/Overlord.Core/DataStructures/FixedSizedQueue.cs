using System;
using System.Collections.Concurrent;

namespace Overlord.Core.DataStructures
{
    public class FixedSizedQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly int _sizeLimit;

        private readonly object _lockObject;

        private readonly Predicate<T> _conditionChecker;
        private int _positiveItemCount;
        private readonly double _positivePercentThresh;

        public FixedSizedQueue(int sizeLimit)
            : this(sizeLimit, t => true)
        {

        }

        public FixedSizedQueue(int sizeLimit, Predicate<T> conditionChecker, double positivePercentThresh = 0.7)
        {
            if (sizeLimit < 1)
            {
                throw new ArgumentException("fixed sized queue size limit not correct.");
            }
            _queue = new ConcurrentQueue<T>();
            _sizeLimit = sizeLimit;
            _lockObject = new object();

            _conditionChecker = conditionChecker;
            _positivePercentThresh = positivePercentThresh;
            _positiveItemCount = 0;
        }

        public void Enqueue(T obj)
        {
            if (_conditionChecker(obj))
            {
                _positiveItemCount++;
            }

            _queue.Enqueue(obj);

            lock (_lockObject)
            {
                T overflow = default(T);
                while (_queue.Count > _sizeLimit && _queue.TryDequeue(out overflow))
                {
                    if (_conditionChecker(overflow))
                    {
                        _positiveItemCount--;
                    }
                }
            }
        }

        public bool IsPositive()
        {
            if (_queue.Count < _sizeLimit)
            {
                return false;
            }

            return _positiveItemCount / (double)_sizeLimit > _positivePercentThresh;
        }

        public int Count()
        {
            return _queue.Count;
        }
        
        public bool Peek(out T t)
        {
            return _queue.TryPeek(out t);
        }
    }
}
