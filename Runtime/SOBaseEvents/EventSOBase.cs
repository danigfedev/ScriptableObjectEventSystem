using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOBaseEvents
{
    public abstract class EventSOBase : ScriptableObject, ISOEventBase, ISOEventRegistry, ISOEventRaiser
    {
        private readonly List<Action> _listeners = new();
        
        public void AddListener(Action listener)
        {
            if (_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} already registered. Aborting registration.");
                return;
            }
            
            _listeners.Add(listener);
        }

        public void RemoveListener(Action listener)
        {
            if (!_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} is not registered. Aborting removal.");
                return;
            }

            _listeners.Remove(listener);
        }

        public void RaiseEvent()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke();
            }
        }
        
        private (string objectName, string methodName) GetListenerInfo(Action listener)
        {
            var objectName = string.Empty;
            var methodName = listener.Method.Name;
                
            if (listener.Target is MonoBehaviour mb)
            {
                objectName = mb.gameObject.name;
            }
            else if (listener.Target != null)
            {
                objectName = listener.Target.ToString();
            }

            return (objectName, methodName);
        }
    }
    
    public abstract class EventSOBase<TArg> : ScriptableObject, ISOEventBase, ISOEventRegistry<TArg>, ISOEventRaiser<TArg>
    {
        private readonly List<Action<TArg>> _listeners = new();
        
        public void AddListener(Action<TArg> listener)
        {
            if (_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} already registered. Aborting registration.");
                return;
            }
            
            _listeners.Add(listener);
        }

        public void RemoveListener(Action<TArg> listener)
        {
            if (!_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} is not registered. Aborting removal.");
                return;
            }

            _listeners.Remove(listener);
        }

        public void RaiseEvent(TArg arg)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke(arg);
            }
        }
        
        private (string objectName, string methodName) GetListenerInfo(Action<TArg> listener)
        {
            var objectName = string.Empty;
            var methodName = listener.Method.Name;
                
            if (listener.Target is MonoBehaviour mb)
            {
                objectName = mb.gameObject.name;
            }
            else if (listener.Target != null)
            {
                objectName = listener.Target.ToString();
            }

            return (objectName, methodName);
        }
    }
    
    public abstract class EventSOBase<TArg1, TArg2> : ScriptableObject, ISOEventBase, ISOEventRegistry<TArg1, TArg2>, ISOEventRaiser<TArg1, TArg2>
    {
        private readonly List<Action<TArg1, TArg2>> _listeners = new();
        
        public void AddListener(Action<TArg1, TArg2> listener)
        {
            if (_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} already registered. Aborting registration.");
                return;
            }
            
            _listeners.Add(listener);
        }

        public void RemoveListener(Action<TArg1, TArg2> listener)
        {
            if (!_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} is not registered. Aborting removal.");
                return;
            }

            _listeners.Remove(listener);
        }

        public void RaiseEvent(TArg1 arg1, TArg2 arg2)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke(arg1, arg2);
            }
        }
        
        private (string objectName, string methodName) GetListenerInfo(Action<TArg1, TArg2> listener)
        {
            var objectName = string.Empty;
            var methodName = listener.Method.Name;
                
            if (listener.Target is MonoBehaviour mb)
            {
                objectName = mb.gameObject.name;
            }
            else if (listener.Target != null)
            {
                objectName = listener.Target.ToString();
            }

            return (objectName, methodName);
        }
    }
    
    public abstract class EventSOBase<TArg1, TArg2, TArg3> : ScriptableObject, ISOEventBase, ISOEventRegistry<TArg1, TArg2, TArg3>, ISOEventRaiser<TArg1, TArg2, TArg3>
    {
        private readonly List<Action<TArg1, TArg2, TArg3>> _listeners = new();
        
        public void AddListener(Action<TArg1, TArg2, TArg3> listener)
        {
            if (_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} already registered. Aborting registration.");
                return;
            }
            
            _listeners.Add(listener);
        }

        public void RemoveListener(Action<TArg1, TArg2, TArg3> listener)
        {
            if (!_listeners.Contains(listener))
            {
                var (objectName, methodName) = GetListenerInfo(listener);
                Debug.LogError($"[ScriptableObjectEvents] Listener {methodName} of GameObject {objectName} is not registered. Aborting removal.");
                return;
            }

            _listeners.Remove(listener);
        }

        public void RaiseEvent(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]?.Invoke(arg1, arg2, arg3);
            }
        }
        
        private (string objectName, string methodName) GetListenerInfo(Action<TArg1, TArg2, TArg3> listener)
        {
            var objectName = string.Empty;
            var methodName = listener.Method.Name;
                
            if (listener.Target is MonoBehaviour mb)
            {
                objectName = mb.gameObject.name;
            }
            else if (listener.Target != null)
            {
                objectName = listener.Target.ToString();
            }

            return (objectName, methodName);
        }
    }
}