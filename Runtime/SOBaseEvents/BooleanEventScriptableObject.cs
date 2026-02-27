using System.Collections.Generic;
using UnityEngine;

namespace SOBaseEvents
{
    [CreateAssetMenu(fileName ="BooleanEventScriptableObject", menuName ="EspidiGames/SO Events/BooleanEventScriptableObject", order = 20)]
    public class BooleanEventScriptableObject : ScriptableObject
    {
        private List<BooleanEventListener> listeners = new List<BooleanEventListener>();

        public void AddListener(BooleanEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                Debug.LogError($"[ScriptableObjectEvents] Listener {listener.name} of GameObject {listener.gameObject.name} already registered. Aborting registration.");
                return;
            }

            listeners.Add(listener);
        }

        public void RemoveListener(BooleanEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                Debug.LogError($"[ScriptableObjectEvents] Listener {listener.name} of GameObject {listener.gameObject.name} is not registered. Aborting removal.");
                return;
            }

            listeners.Remove(listener);
        }

        public void RiseEvent(bool arg1) //Type1 arg1, Type2 arg2, Type3 arg3...
        {
            foreach(var listener in listeners)
            {
                listener.RiseEvent(arg1); //arg1, arg2, arg3...
            }
        }
    }
}