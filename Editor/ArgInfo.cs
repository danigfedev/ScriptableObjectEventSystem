using UnityEngine;

namespace EG.ScriptableObjectSystem.Editor
{
    public class ArgInfo
    {
        public EventSupportedArgs SupportedType { get; private set; }
        public string ArgType { get; private set; }
        public string ArgNamespace { get; private set; }
        
        public static ArgInfo CreateStandardArg()
        {
            return new ArgInfo(EventSupportedArgs.Int);
        }
        
        public static ArgInfo CreateStandardArg(EventSupportedArgs supportedType)
        {
            return new ArgInfo(supportedType);
        }

        public static ArgInfo CreateCustomTypeArg(string argType, string argNamespace)
        {
            return new ArgInfo(EventSupportedArgs.Custom, argType, argNamespace);
        }

        public void UpdateInfo(EventSupportedArgs supportedType, string customArgType = null, string argNamespace = null)
        {
            if (supportedType != EventSupportedArgs.Custom)
            {
                if (!string.IsNullOrEmpty(customArgType))
                {
                    Debug.LogWarning("You´re marking the Arg as standard Type. Custom Arg Type and its namespace are not required and will be ignored.");
                }
                
                SetStandardType(supportedType);
            }
            else
            {
                if (string.IsNullOrEmpty(customArgType))
                {
                    Debug.LogWarning("You´re updating the Arg Info with Custom type. You must provide the Type and, optionally, its namespace");
                }
                
                SetCustomType(supportedType, customArgType, argNamespace);
            }
        }

        public void ForceCustomArg()
        {
            SetCustomType(EventSupportedArgs.Custom, null, null);
        }

        private ArgInfo(EventSupportedArgs supportedType)
        {
            SetStandardType(supportedType);
        }

        private ArgInfo(EventSupportedArgs supportedType, string customArgType, string argNamespace)
        {
            SetCustomType(supportedType, customArgType, argNamespace);
        }

        private void SetStandardType(EventSupportedArgs supportedType)
        {
            SupportedType = supportedType;
            ArgType = SupportedType.ToString().ToLower();
        }
        
        private void SetCustomType(EventSupportedArgs supportedType, string customArgType, string argNamespace)
        {
            SupportedType = supportedType;
            ArgType = customArgType;
            ArgNamespace = argNamespace;
        }
    }
}