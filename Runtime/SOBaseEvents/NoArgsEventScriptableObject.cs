using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="NoArgsEventScriptableObject", menuName ="EspidiGames/SO Events/NoArgsEventScriptableObject", order = 20)]
public class NoArgsEventScriptableObject : ScriptableObject
{
    private List<NoArgsEventListener> listeners = new List<NoArgsEventListener>();


    public void AddListener(NoArgsEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            Debug.LogError($"[ScriptableObjectEvents] Listener {listener.name} of GameObject {listener.gameObject.name} already registered. Aborting registration.");
            return;
        }

        listeners.Add(listener);
    }

    public void RemoveListener(NoArgsEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            Debug.LogError($"[ScriptableObjectEvents] Listener {listener.name} of GameObject {listener.gameObject.name} is not registered. Aborting removal.");
            return;
        }

        listeners.Remove(listener);
    }

    public void RiseEvent()
    {
        foreach(var listener in listeners)
        {
            listener.RiseEvent();
        }
    }
}
