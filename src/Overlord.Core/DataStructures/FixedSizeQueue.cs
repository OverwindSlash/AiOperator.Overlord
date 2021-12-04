using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Overlord.Core.DataStructures
{
    public class FixedSizeQueue<T> : IEnumerable<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly int _sizeLimit;

        private readonly object _lockObject;

        // predicate which check whether item if positive or not.
        private readonly Predicate<T> _conditionChecker;
        private int _positiveItemCount;
        private readonly double _positivePercentThresh;

        public FixedSizeQueue(int sizeLimit)
            : this(sizeLimit, _ => true)
        {

        }

        public FixedSizeQueue(int sizeLimit, Predicate<T> conditionChecker, double positivePercentThresh = 0.7)
        {
            if (sizeLimit < 1)
            {
                throw new ArgumentException("queue size limit not correct.");
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
                while (_queue.Count > _sizeLimit && _queue.TryDequeue(out var overflow))
                {
                    if (_conditionChecker(overflow))
                    {
                        _positiveItemCount--;
                    }
                }
            }
        }

        public T Dequeue()
        {
            _queue.TryDequeue(out T value);
            return value;
        }

        public bool IsPositive()
        {
            // The overall judgment can only be carried out when the queue is full
            if (_queue.Count < _sizeLimit)
            {
                return false;
            }

            return _positiveItemCount / (double)_sizeLimit >= _positivePercentThresh;
        }

        public int Count()
        {
            return _queue.Count;
        }
        
        public bool Peek(out T t)
        {
            return _queue.TryPeek(out t);
        }
        
        #region Enumerator
        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
