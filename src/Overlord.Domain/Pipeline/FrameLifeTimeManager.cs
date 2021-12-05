using System;
using System.Collections.Generic;
using System.Linq;
using Overlord.Core.DataStructures;
using Overlord.Core.Entities.Frame;
using Overlord.Domain.Event;

namespace Overlord.Domain.Pipeline
{
    public class FrameLifeTimeManager : CircularList<FrameInfo>, IObservable<ObjectExpiredEvent>, IObservable<FrameExpiredEvent>
    {
        private readonly Dictionary<string, HashSet<int>> _idInWhichSlots;

        private readonly List<IObserver<ObjectExpiredEvent>> _objExpiredObservers;
        private readonly List<IObserver<FrameExpiredEvent>> _frmExpiredObservers;

        public FrameLifeTimeManager(int frameCount) : base(frameCount)
        {
            if (frameCount < 1)
            {
                throw new ArgumentException("invalid frame count.");
            }
            
            _idInWhichSlots = new Dictionary<string, HashSet<int>>();

            _objExpiredObservers = new List<IObserver<ObjectExpiredEvent>>();
            _frmExpiredObservers = new List<IObserver<FrameExpiredEvent>>();
        }

        #region Observer related
        public IDisposable Subscribe(IObserver<ObjectExpiredEvent> observer)
        {
            if (!_objExpiredObservers.Contains(observer))
                _objExpiredObservers.Add(observer);

            return new Unsubscriber<ObjectExpiredEvent>(_objExpiredObservers, observer);
        }

        public IDisposable Subscribe(IObserver<FrameExpiredEvent> observer)
        {
            if (!_frmExpiredObservers.Contains(observer))
                _frmExpiredObservers.Add(observer);

            return new Unsubscriber<FrameExpiredEvent>(_frmExpiredObservers, observer);
        }
        
        private class Unsubscriber<T> : IDisposable
        {
            private readonly List<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
        
        private void NotifyObservers(ObjectExpiredEvent cleanUpEvent)
        {
            foreach (IObserver<ObjectExpiredEvent> observer in _objExpiredObservers)
            {
                observer.OnNext(cleanUpEvent);
            }
        }

        private void NotifyObservers(FrameExpiredEvent cleanUpEvent)
        {
            foreach (IObserver<FrameExpiredEvent> observer in _frmExpiredObservers)
            {
                observer.OnNext(cleanUpEvent);
            }
        }
        #endregion

        public void AddFrameInfo(FrameInfo newFrameInfo)
        {
            int currentIndex = this.CurrentIndex;

            // Add the new FrameInfo to the current Slot and update _idInWhichSlots.
            // this function will increase CurrentIndex
            this.AddItem(newFrameInfo);
            
            AddNewIdsToIdInWhichSlots(newFrameInfo, currentIndex);
        }

        protected override void CleanUpSlot(int currentIndex)
        {
            // Get FrameInfo to be expired.
            FrameInfo lastFrameInfo = this.GetItem(currentIndex);
            if (lastFrameInfo != null)
            {
                // Remove ids of expired FrameInfo from _idInWhichSlots.
                RemoveExpiredIdsFromIdInWhichSlots(lastFrameInfo, currentIndex);

                foreach (TrafficObjectInfo toi in lastFrameInfo.TrafficObjectInfos)
                {
                    int existenceCount = GetExistenceCountById(toi.Id);

                    // If the existenceCount = 0, it means that the object with the specified id has not
                    // reappeared in this lifecycle, and the id can be deleted directly.
                    if (existenceCount == 0)
                    {
                        _idInWhichSlots.Remove(toi.Id);
                        NotifyObservers(new ObjectExpiredEvent(toi));   // Notify object expired.
                    }
                }

                // Notify frame expired.
                NotifyObservers(new FrameExpiredEvent(lastFrameInfo));

                // Clean up last frame.
                lastFrameInfo.Dispose();
            }
        }

        private void RemoveExpiredIdsFromIdInWhichSlots(FrameInfo lastFrameInfo, int currentIndex)
        {
            if (lastFrameInfo.TrafficObjectInfos == null)
            {
                return;
            }

            foreach (TrafficObjectInfo toi in lastFrameInfo.TrafficObjectInfos)
            {
                if (!_idInWhichSlots.ContainsKey(toi.Id))
                {
                    continue;
                }

                HashSet<int> slotIds = _idInWhichSlots[toi.Id];
                if (slotIds != null)
                {
                    slotIds.Remove(currentIndex);
                }
            }
        }

        private void AddNewIdsToIdInWhichSlots(FrameInfo newFrameInfo, int currentIndex)
        {
            if (newFrameInfo.TrafficObjectInfos == null)
            {
                return;
            }

            foreach (TrafficObjectInfo toi in newFrameInfo.TrafficObjectInfos)
            {
                if (!_idInWhichSlots.ContainsKey(toi.Id))
                {
                    _idInWhichSlots.Add(toi.Id, new HashSet<int>());
                }

                HashSet<int> slotIds = _idInWhichSlots[toi.Id];
                slotIds.Add(currentIndex);
            }
        }

        public int GetExistenceCountById(string id)
        {
            if (!_idInWhichSlots.ContainsKey(id))
            {
                // Programming error. Skip this id.
                return 0;
            }

            HashSet<int> slotSet = _idInWhichSlots[id];
            if (slotSet == null)
            {
                // Programming error. Skip this id.
                _idInWhichSlots.Remove(id);
                return 0;
            }

            return slotSet.Count;
        }

        public FrameInfo GetFrameInfoByIndex(int index)
        {
            if (index < 0 || index > this.Count() - 1)
            {
                throw new ArgumentException("index out of range.");
            }

            return this.GetItem(index);
        }

        public int GetFrameCount()
        {
            return this.Count();
        }

        public int GetCurrentIndex()
        {
            return this.CurrentIndex;
        }
    }
}
