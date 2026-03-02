using System;
using SOBaseEvents.Refactor.Impl;
using UnityEngine;

namespace SOBaseEvents.Refactor
{
    public abstract class EventListener<TEvent, TArg> : EventListenerBase where TEvent : EventSOBase<TArg>
    {
        public event Action<TArg> OnEventRaised;
        
        protected TEvent _typedEvent => _baseEvent as TEvent;

        private void OnEnable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.AddListener(OnEventRaised);
            }
        }

        private void OnDisable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.RemoveListener(OnEventRaised);
            }
        }
    }
}