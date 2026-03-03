using UnityEditor;
using System;
using UnityEngine;

namespace Editor.EditorUI
{
    public class BaseEventListenerCustomInspector : UnityEditor.Editor
    {
        protected const string EventSOVariableName = "_baseEvent";
        
        protected SerializedProperty _baseEventProp;
        
        protected virtual void OnEnable()
        {
            _baseEventProp = serializedObject.FindProperty(EventSOVariableName);
        }
        
        protected void DrawEventInspector(string label, string tooltip, Func<UnityEngine.Object, bool> validationLogic)
        {
            serializedObject.Update();
            // EditorGUILayout.Space();
            // EditorGUILayout.LabelField("Event Configuration", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var newObj = EditorGUILayout.ObjectField(new GUIContent(label, tooltip), _baseEventProp.objectReferenceValue, typeof(ScriptableObject), false);

            if (EditorGUI.EndChangeCheck())
            {
                if (newObj == null || validationLogic(newObj))
                {
                    _baseEventProp.objectReferenceValue = newObj;
                }
                else
                {
                    EditorUtility.DisplayDialog("Compatibility Error",
                        "The selected Asset does not implement the required interface.", "Ok");
                }
            }

            DrawPropertiesExcluding(serializedObject, "m_Script", EventSOVariableName);
            serializedObject.ApplyModifiedProperties();
        }
    }
}