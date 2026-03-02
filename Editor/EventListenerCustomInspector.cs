using UnityEditor;
using SOBaseEvents.Refactor;
using System;
using UnityEngine;

namespace EG.ScriptableObjectSystem.Editor
{
    [CustomEditor(typeof(EventListener<,>), true)]
    [CanEditMultipleObjects]
    public class EventListenerCustomInspector: UnityEditor.Editor
    {
        private const string EventSOVariableName = "_baseEvent";
        
        SerializedProperty _baseEventProp;
        Type _tArg;
        
        private void OnEnable()
        {
            _baseEventProp = serializedObject.FindProperty(EventSOVariableName);
            var listenerType = target.GetType();
            
            var baseType = listenerType;
            while (baseType != null 
                   && (!baseType.IsGenericType 
                       || baseType.GetGenericTypeDefinition() != typeof(EventListener<,>)))
            {
                baseType = baseType.BaseType;
            }

            if (baseType != null)
            {
                _tArg = baseType.GetGenericArguments()[1];
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Configuración del Evento", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
        
            var newObj = EditorGUILayout.ObjectField(
                new GUIContent("Event Asset", $"Must implement ISOEventRegistry<{_tArg.Name}>"),
                _baseEventProp.objectReferenceValue,
                typeof(ScriptableObject),
                false
            );

            if (EditorGUI.EndChangeCheck())
            {
                if (newObj == null)
                {
                    _baseEventProp.objectReferenceValue = null;
                }
                else
                {
                    var implementsBase = newObj is ISOEventBase;
                    
                    var registryInterface = typeof(ISOEventRegistry<>).MakeGenericType(_tArg);
                    var implementsRegistry = registryInterface.IsAssignableFrom(newObj.GetType());

                    if (implementsBase && implementsRegistry)
                    {
                        _baseEventProp.objectReferenceValue = newObj;
                    }
                    else
                    {
                        var errorMsg = $"Asset '{newObj.name}' is not valid.\n\n";
                        if (!implementsBase)
                        {
                            errorMsg += "- Does not implement ISOEventBase.\n";
                        }

                        if (!implementsRegistry)
                        {
                            errorMsg += $"- Does not implement ISOEventRegistry<{_tArg.Name}>.\n";
                        }

                        EditorUtility.DisplayDialog("Compatibility Error", errorMsg, "Ok");
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}