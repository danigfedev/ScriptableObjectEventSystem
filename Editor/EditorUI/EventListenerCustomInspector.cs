using System;
using SOBaseEvents.Refactor;
using UnityEditor;

namespace Editor.EditorUI
{
    [CustomEditor(typeof(EventListener<>), true)]
    [CanEditMultipleObjects]
    public class EventListenerCustomInspector : BaseEventListenerCustomInspector
    {
        public override void OnInspectorGUI()
        {
            DrawEventInspector("Event Asset (Void)",
                "Must implement ISOEventRegistry",
                obj => ValidationLogic(obj));
        }

        protected override bool ValidationLogic(Object obj)
        {
            return obj is ISOEventBase && obj is ISOEventRegistry;
        }
    }
}