using System;
using System.Collections;
using System.Collections.Generic;

namespace Overlord.Core.DataStructures
{
    public class CircularList<T> : IEnumerable<T>
    {
        private readonly List<T> _slots;
        private readonly int _slotCount;
        private int _currentIndex;

        public int CurrentIndex => _currentIndex;

        public CircularList(int slotCount)
        {
            _slotCount = slotCount;
            _slots = new List<T>(_slotCount);
            for (int i = 0; i < _slotCount; i++)
            {
                _slots.Add(default(T));
            }
            
            _currentIndex = 0;
        }

        public void AddItem(T item)
        {
            if (item == null) { return; }

            CleanUpSlot(_currentIndex);

            _slots[_currentIndex++] = item;

            _currentIndex = CalculateSlotIndex(_currentIndex);
        }

        public T GetItem(int index)
        {
            int slotIndex = CalculateSlotIndex(index);

            return _slots[slotIndex];
        }

        protected virtual int CalculateSlotIndex(int index)
        {
            if (index < 0) { return 0; }

            return index % _slotCount;
        }

        protected virtual void CleanUpSlot(int currentIndex)
        {
            // subclass needs to override this function to perform cleanup.
            T item = _slots[_currentIndex];
            if (item is IDisposable)
            {
                IDisposable disposable = item as IDisposable;
                disposable.Dispose();
            }

            _slots[_currentIndex] = default(T);
        }

        #region Enumerator

        public IEnumerator<T> GetEnumerator()
        {
            return _slots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}