using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOBaseEvents.Refactor
{
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
}