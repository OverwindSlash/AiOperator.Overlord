using System;
using System.Collections;
using System.Collections.Generic;

namespace Overlord.Core.DataStructures
{
    // Circular list for analysis slide window.
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

            // 1. clean up current slot.
            CleanUpSlot(_currentIndex);

            // 2. add new item to current slot.
            _slots[_currentIndex] = item;

            // 3. calculate new slot index of next item.
            _currentIndex = CalculateSlotIndex(++_currentIndex);
        }

        public T GetItem(int index)
        {
            int slotIndex = CalculateSlotIndex(index);

            return _slots[slotIndex];
        }

        private int CalculateSlotIndex(int index)
        {
            if (index < 0) { return 0; }

            return index % _slotCount;
        }

        protected virtual void CleanUpSlot(int currentIndex)
        {
            // subclass can override this function to perform customize cleanup.
            T item = _slots[_currentIndex];
            if (item is IDisposable disposable)
            {
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