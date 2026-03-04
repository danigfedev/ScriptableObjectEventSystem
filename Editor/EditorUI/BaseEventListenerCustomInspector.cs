using UnityEditor;
using System;
using System.Linq;
using UnityEngine;

namespace Editor.EditorUI
{
    public abstract class BaseEventListenerCustomInspector : UnityEditor.Editor
    {
        protected const string EventSOVariableName = "_baseEvent";
        
        protected SerializedProperty _baseEventProp;
        protected Type[] _tArgs;
        protected string _types;
        
        protected abstract bool ValidateEventType(System.Object obj);
        
        protected virtual void OnEnable()
        {
            _baseEventProp = serializedObject.FindProperty(EventSOVariableName);
            _types = string.Empty;
        }
        
        protected void DrawEventInspector(string label, string tooltip, Func<UnityEngine.Object, bool> validationLogic)
        {
            serializedObject.Update();

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
        
        protected void GetArgTypes(Type type)
        {
            if (type != null)
            {
                var allArgs = type.GetGenericArguments();
                
                //Skipping the first one. Not sure if this is always applicable
                _tArgs = new Type[allArgs.Length - 1];
                for (int i = 1; i< allArgs.Length; i++)
                {
                    _tArgs[i - 1] = allArgs[i];
                    _types += _tArgs[i-1].Name;
                    if (i < allArgs.Length - 1)
                    {
                        _types +=", ";
                    }
                }
            }
        }
        
        protected bool ValidateArgCountAndTypes(Type @interface)
        {
            var args = @interface.GetGenericArguments();
            
            if (args.Length == _tArgs.Length)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] != _tArgs[i])
                    {
                        return false;
                    }
                }
                
                return true;
            }

            return false;
        }
    }
}