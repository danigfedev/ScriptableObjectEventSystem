using System;
using UnityEngine;

namespace SOBaseEvents
{
    public abstract class EventListener<TEvent> : MonoBehaviour where TEvent : class, ISOEventRegistry
    {
        public event Action OnEventRaised;
        
        [SerializeField] protected ScriptableObject _baseEvent;  
        
        protected TEvent _typedEvent => _baseEvent as TEvent;

        private void OnEnable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.AddListener(OnEventRaiseRequested);
            }
        }

        private void OnDisable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.RemoveListener(OnEventRaiseRequested);
            }
        }

        private void OnEventRaiseRequested()
        {
            OnEventRaised?.Invoke();
        }
    }
    
    public abstract class EventListener<TEvent, TArg> : MonoBehaviour where TEvent : class, ISOEventRegistry<TArg>
    {
        public event Action<TArg> OnEventRaised;
        
        [SerializeField] protected ScriptableObject _baseEvent;  
        
        protected TEvent _typedEvent => _baseEvent as TEvent;

        private void OnEnable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.AddListener(OnEventRaiseRequested);
            }
        }

        private void OnDisable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.RemoveListener(OnEventRaiseRequested);
            }
        }
        
        private void OnEventRaiseRequested(TArg value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
    
    public abstract class EventListener<TEvent, TArg1, TArg2> : MonoBehaviour where TEvent : class, ISOEventRegistry<TArg1, TArg2>
    {
        public event Action<TArg1, TArg2> OnEventRaised;
        
        [SerializeField] protected ScriptableObject _baseEvent;  
        
        protected TEvent _typedEvent => _baseEvent as TEvent;

        private void OnEnable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.AddListener(OnEventRaiseRequested);
            }
        }

        private void OnDisable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.RemoveListener(OnEventRaiseRequested);
            }
        }
        
        private void OnEventRaiseRequested(TArg1 value1, TArg2 value2)
        {
            OnEventRaised?.Invoke(value1, value2);
        }
    }
    
    public abstract class EventListener<TEvent, TArg1, TArg2, TArg3> : MonoBehaviour where TEvent : class, ISOEventRegistry<TArg1, TArg2, TArg3>
    {
        public event Action<TArg1, TArg2, TArg3> OnEventRaised;
        
        [SerializeField] protected ScriptableObject _baseEvent;  
        
        protected TEvent _typedEvent => _baseEvent as TEvent;

        private void OnEnable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.AddListener(OnEventRaiseRequested);
            }
        }

        private void OnDisable()
        {
            if (_typedEvent != null)
            {
                _typedEvent.RemoveListener(OnEventRaiseRequested);
            }
        }

        private void OnEventRaiseRequested(TArg1 value1, TArg2 value2, TArg3 value3)
        {
            OnEventRaised?.Invoke(value1, value2, value3);
        }
    }
}