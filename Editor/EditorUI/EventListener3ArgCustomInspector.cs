using SOBaseEvents.Refactor;
using UnityEditor;
using System;

namespace Editor.EditorUI
{
    [CustomEditor(typeof(EventListener<,,,>), true)]
    [CanEditMultipleObjects]
    public class EventListener3ArgCustomInspector : BaseEventListenerCustomInspector
    {
        Type _tArg;
        
        private void OnEnable()
        {
            base.OnEnable();
            var type = target.GetType();

            while (type != null && (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(EventListener<,,,>)))
            {
                type = type.BaseType;
            }

            if (type != null)
            {
                _tArg = type.GetGenericArguments()[1];
            }
        }
        
        public override void OnInspectorGUI()
        {
            DrawEventInspector("Event Asset",
                $"Must implement ISOEventRegistry<{(_tArg != null ? _tArg.Name : "T")}>",
                obj => ValidationLogic(obj));
        }

        protected override bool ValidationLogic(Object obj)
        {
            var registryInterface = typeof(ISOEventRegistry<,,>).MakeGenericType(_tArg);
            return obj is ISOEventBase && registryInterface.IsAssignableFrom(obj.GetType());
        }
    }
}