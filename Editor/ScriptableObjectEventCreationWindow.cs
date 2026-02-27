using System.Collections;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace EG.ScriptableObjectSystem.Editor
{
    public class ScriptableObjectEventCreationWindow : EditorWindow
    {
        private const string EventSONamePattern = "{0}EventScriptableObject";
        private const string EventListenerNamePattern = "{0}EventListener";
        private const int MaxArgs = 3;
        
        private static Vector2 InitialWindowDimensions = new Vector2(300, 190);
        private static float WindowArgHeightDelta = 50;
        private static float WindowCustomArgHeightDelta = 40;

        private static ScriptableObjectEventCreationWindow _window;
        
        private ArgInfo[] _argList = new ArgInfo[0];
        private string _eventName = "";
        private string _namespace;
        private string _eventSOName;
        private string _eventListenerName;
        private int _argCount = 0;
        private string _customType;
        private string _customTypeNamespace;
        
        public static void OpenWindow()
        {
            //Calling this opens window normally (non-modal)
            _window = GetWindow<ScriptableObjectEventCreationWindow>(true, "Select Randomized Selected Objects");
            SetWindowSize(InitialWindowDimensions);
            EditorCoroutineUtility.StartCoroutineOwnerless(_window.MakeWindowModal());
        }

        /// <summary>
        /// Coroutine that skips one editor frame and then makes Window modal.
        /// Necessary to open window from Project panel (right click -> Create/EspidiGames/SO Events / Open window)
        ///For whatever reason, opening from that context menu left the window completely blank
        /// (it was working from unity menus though))
        /// </summary>
        private IEnumerator MakeWindowModal()
        {
            yield return null; //Wait just for one editor frame

            this.ShowModalUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            RenderEventNameAndNamespaceSection();
            EditorGUILayout.Space();
            RenderArgumentsSelectorSection();
            GUILayout.FlexibleSpace();
            RenderOutputFilesSection();
            RenderCreateAssetsButton();
        }
        
        void OnInspectorUpdate()
        {
            Repaint();
        }
        
        private void RenderEventNameAndNamespaceSection()
        {
            _eventName = EditorGUILayout.TextField("Event name:", _eventName);
            _namespace = EditorGUILayout.TextField("Namespace (Optional):", _namespace);
        }

        private void RenderArgumentsSelectorSection()
        {
            var newArgCount = EditorGUILayout.IntSlider("Arg count", _argCount, 0, MaxArgs);

            if (newArgCount != _argCount)
            {
                _argCount = newArgCount;
                System.Array.Resize(ref _argList, _argCount);
            }
            
            for(int argIndex = 0; argIndex < _argCount; argIndex++)
            {
                GUILayout.Space(10);
                RenderArgumentSelector(argIndex);
            }
            
            AdjustWindowHeight();
        }

        private void RenderArgumentSelector(int argIndex)
        {
            _argList[argIndex] ??= ArgInfo.CreateStandardArg();

            GUILayout.Label($"Argument {argIndex + 1}", EditorStyles.boldLabel);
            var newType = (EventSupportedArgs)EditorGUILayout.EnumPopup("Argument Type", _argList[argIndex].SupportedType);
            
            string newCustomType = null;
            string newCustomTypeNamespace = null;
                
            if (newType == EventSupportedArgs.Custom)
            {
                if (_argList[argIndex].SupportedType != EventSupportedArgs.Custom)
                {
                    _argList[argIndex].ForceCustomArg();
                }
                    
                newCustomType = EditorGUILayout.TextField("Type:", _argList[argIndex].ArgType);
                newCustomTypeNamespace = EditorGUILayout.TextField("Namespace:", _argList[argIndex].ArgNamespace);
            }
                
            if (newType != _argList[argIndex].SupportedType 
                || newCustomType != _argList[argIndex].ArgType 
                || newCustomTypeNamespace != _argList[argIndex].ArgNamespace)
            {
                _argList[argIndex].UpdateInfo(newType, newCustomType, newCustomTypeNamespace);
            }
        }

        private void RenderOutputFilesSection()
        {
            _eventSOName = string.Format(EventSONamePattern, _eventName);
            _eventListenerName = string.Format(EventListenerNamePattern, _eventName);
            
            var messageType = string.IsNullOrWhiteSpace(_eventName) ? MessageType.Warning : MessageType.None;
            var baseMessageType = $"Output files: \n\n" +
                                  $"- {_eventSOName}.cs\n" +
                                  $"- {_eventListenerName}.cs";
            var warningMessage = "\n\n" +
                                 $"Recommended adding an event name for clarity";

            var helpMessage = messageType == MessageType.None ? baseMessageType : baseMessageType + warningMessage;
            EditorGUILayout.HelpBox(helpMessage, messageType);
        }
        
        private void RenderCreateAssetsButton()
        {
            if (GUILayout.Button("Create SO event scripts"))
            {
                AssetCreationMenu.CreateSOEventScripts(_eventSOName, _eventListenerName, _namespace, _argList);
                Close();
            }
        }
        
        private void AdjustWindowHeight()
        {
            var customArgCount = _argList.Where(arg => arg.SupportedType == EventSupportedArgs.Custom).Count();
            var argsDeltaHeight = WindowArgHeightDelta * _argCount;
            var rotalDeltaHeight = argsDeltaHeight + WindowCustomArgHeightDelta * customArgCount;
            
            var newWindowSize = InitialWindowDimensions + Vector2.up * rotalDeltaHeight;
            SetWindowSize(newWindowSize);
        }
        
        private static void SetWindowSize(Vector2 windowDimensions)
        {
            _window.minSize = windowDimensions;
            _window.maxSize = windowDimensions;
        }
    }
}
