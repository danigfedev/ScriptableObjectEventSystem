using UnityEditor;
using System;
using SOBaseEvents;

namespace Editor.EditorUI
{
    [CustomEditor(typeof(EventListener<,,>), true)]
    [CanEditMultipleObjects]
    public class EventListener2ArgCustomInspector : BaseEventListenerCustomInspector
    {
        Type[] _tArgs;
        private string _types;
        
        private void OnEnable()
        {
            base.OnEnable();
            var type = target.GetType();

            while (type != null && (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(EventListener<,,>)))
            {
                type = type.BaseType;
            }

            GetArgTypes(type);
        }
        
        public override void OnInspectorGUI()
        {
            DrawEventInspector("Event Asset",
                $"Must implement ISOEventRegistry<{(_tArgs != null ? _types : "T")}>",
                obj => ValidateEventType(obj));
        }

        protected override bool ValidateEventType(Object obj)
        {
            if (!(obj is ISOEventBase))
            {
                return false;
            }

            var objType = obj.GetType();
            var argsValidated = false;
            
            foreach (var iface in objType.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(ISOEventRegistry<,>))
                {
                    argsValidated |= ValidateArgCountAndTypes(iface);
                }
            }
            
            return argsValidated;
        }
    }
}