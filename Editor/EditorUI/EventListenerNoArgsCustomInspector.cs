using System;
using SOBaseEvents;
using UnityEditor;

namespace Editor.EditorUI
{
    [CustomEditor(typeof(EventListener<>), true)]
    [CanEditMultipleObjects]
    public class EventListenerNoArgsCustomInspector : BaseEventListenerCustomInspector
    {
        public override void OnInspectorGUI()
        {
            DrawEventInspector("Event Asset (Void)",
                "Must implement ISOEventRegistry",
                obj => ValidateEventType(obj));
        }

        protected override bool ValidateEventType(Object obj)
        {
            return obj is ISOEventBase && obj is ISOEventRegistry;
        }
    }
}