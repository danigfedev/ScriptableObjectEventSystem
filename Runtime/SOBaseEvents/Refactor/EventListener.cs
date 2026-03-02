using System;
using UnityEngine;

namespace SOBaseEvents.Refactor
{
    public abstract class EventListener<TEvent, TArg> : MonoBehaviour where TEvent : class, ISOEventRegistry<TArg>
    {
        public event Action<TArg> OnEventRaised;
        
        [SerializeField] protected ScriptableObject _baseEvent;  
        
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