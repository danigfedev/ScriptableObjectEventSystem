using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StringEventScriptableObject", menuName ="EspidiGames/SO Events/StringEventScriptableObject", order = 20)]
public class StringEventScriptableObject : ScriptableObject
{
    private List<StringEventListener> listeners = new List<StringEventListener>();


    public void AddListener(StringEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            Debug.LogError($"[ScriptableObjectEvents] Listener {listener.name} of GameObject {listener.gameObject.name} already registered. Aborting registration.");
            return;
        }

        listeners.Add(listener);
    }

    public void RemoveListener(StringEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            Debug.LogError($"[ScriptableObjectEvents] Listener {listener.name} of GameObject {listener.gameObject.name} is not registered. Aborting removal.");
            return;
        }

        listeners.Remove(listener);
    }

    public void RiseEvent(string arg1) //Type1 arg1, Type2 arg2, Type3 arg3...
    {
        foreach(var listener in listeners)
        {
            listener.RiseEvent(arg1); //arg1, arg2, arg3...
        }
    }
}
