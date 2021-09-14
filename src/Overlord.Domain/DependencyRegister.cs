using System;
using System.Collections.Generic;
using Overlord.Domain.Interfaces;

namespace Overlord.Domain
{
    public class DependencyRegister
    {
        private static readonly DependencyRegister _instance = new DependencyRegister();
        
        private readonly List<IObjectDetector> _detectors;
        private readonly List<IMultiObjectTracker> _trackers;
        private readonly List<ITrafficEventGenerator> _eventGenerators;
        private readonly List<ITrafficEventPublisher> _eventPublishers;
        private readonly List<ISpeeder> _speeders;

        private DependencyRegister()
        {
            _detectors = new List<IObjectDetector>();
            _trackers = new List<IMultiObjectTracker>();
            _eventGenerators = new List<ITrafficEventGenerator>();
            _eventPublishers = new List<ITrafficEventPublisher>();
            _speeders = new List<ISpeeder>();
        }

        public static DependencyRegister GetInstance()
        {
            return _instance;
        }

        public void Reset()
        {
            _detectors.Clear();
            _trackers.Clear();
            _eventGenerators.Clear();
            _eventPublishers.Clear();
            _speeders.Clear();
        }

        public void AddObjectDetector(IObjectDetector detector)
        {
            if (detector == null)
            {
                throw new ArgumentException("invalid object detector.");
            }
            
            _detectors.Add(detector);
        }

        public void AddMultiObjectTracker(IMultiObjectTracker tracker)
        {
            if (tracker == null)
            {
                throw new ArgumentException("invalid object tracker.");
            }

            _trackers.Add(tracker);
        }
        
        public void AddEventGenerator(ITrafficEventGenerator eventGenerator)
        {
            if (eventGenerator == null)
            {
                throw new ArgumentException("invalid event generator.");
            }
            
            _eventGenerators.Add(eventGenerator);
        }
        
        public void AddEventPublisher(ITrafficEventPublisher eventPublisher)
        {
            if (eventPublisher == null)
            {
                throw new ArgumentException("invalid event publisher.");
            }
            
            _eventPublishers.Add(eventPublisher);
        }
        
        public void AddSpeeder(ISpeeder speeder)
        {
            if (speeder == null)
            {
                throw new ArgumentException("invalid speeder.");
            }
            
            _speeders.Add(speeder);
        }

        public IObjectDetector GetDetector(int pipelineIndex)
        {
            CheckPipelineIndex(pipelineIndex);

            return _detectors[pipelineIndex];
        }

        public IMultiObjectTracker GetTracker(int pipelineIndex)
        {
            CheckPipelineIndex(pipelineIndex);

            return _trackers[pipelineIndex];
        }

        public ITrafficEventGenerator GetEventGenerator(int pipelineIndex)
        {
            CheckPipelineIndex(pipelineIndex);
            
            return _eventGenerators[pipelineIndex];
        }

        public ITrafficEventPublisher GetEventPublisher(int pipelineIndex)
        {
            CheckPipelineIndex(pipelineIndex);
            
            return _eventPublishers[pipelineIndex];
        }

        public ISpeeder GetSpeeder(int pipelineIndex)
        {
            CheckPipelineIndex(pipelineIndex);
            
            return _speeders[pipelineIndex];
        }
        
        private void CheckPipelineIndex(int pipelineIndex)
        {
            if (pipelineIndex > _detectors.Count - 1)
            {
                throw new ArgumentException("wrong pipeline index.");
            }
        }
    }
}