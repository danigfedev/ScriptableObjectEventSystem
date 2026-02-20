using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectEventCreationWindow : EditorWindow
{
    private enum ToggleIndices
    {
        NoArgs = 0,
        Int,
        Float,
        Bool,
        String,
        Custom
    }

    private const string EventSONamePattern = "SO{0}Event";
    private const string EventListenerNamePattern = "SO{0}EventListener";
    
    private bool _useTypeAsName = false;
    private bool[] _argsToggle = new bool[6];
    private bool[] _argsTogglePreviousState = new bool[6];
    private ToggleIndices _currentActiveToggle;
    private string _eventName = "";
    private string customTypes = "";
    private string _eventSOName;
    private string _eventListenerName;
    
    public static void OpenWindow()
    {
        //Calling this opens window normally (non-modal)
        var window = GetWindow<ScriptableObjectEventCreationWindow>(true, "Select Randomized Selected Objects");

        var windowDimensions = new Vector2(300, 400);
        window.minSize = windowDimensions;
        window.maxSize = windowDimensions;

        window._argsToggle[(int)ToggleIndices.NoArgs] = true;
        window._argsTogglePreviousState[(int)ToggleIndices.NoArgs] = true;
        window._currentActiveToggle = ToggleIndices.NoArgs;

        EditorCoroutineUtility.StartCoroutineOwnerless(window.MakeWindowModal());
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
        RenderEventNameSection();
        EditorGUILayout.Space(20);
        RenderArgumentsToggleSection();
        UpdateToggles();
        EditorGUILayout.Space(20);
        RenderOutputFilesSection();
        GUILayout.FlexibleSpace();
        RenderCreateAssetsButton();
    }
    
    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void RenderEventNameSection()
    {
        _useTypeAsName = EditorGUILayout.ToggleLeft("Use argument Type as name", _useTypeAsName);
        
        if (_argsToggle[(int)ToggleIndices.Custom])
        {
            EditorGUILayout.HelpBox($"Custom type is selected. It is recommended to add a custom name manually.", MessageType.Warning);
        }
        
        _eventName = EditorGUILayout.TextField("Event name:", _eventName);
    }
    
    private void RenderArgumentsToggleSection()
    {
        EditorGUILayout.LabelField("Event Arguments");
        EditorGUILayout.Space(5);
        
        _argsToggle[(int)ToggleIndices.NoArgs] = EditorGUILayout.ToggleLeft("No Argument", _argsToggle[(int)ToggleIndices.NoArgs]);
        _argsToggle[(int)ToggleIndices.Int] = EditorGUILayout.ToggleLeft("int", _argsToggle[(int)ToggleIndices.Int]);
        _argsToggle[(int)ToggleIndices.Float] = EditorGUILayout.ToggleLeft("float", _argsToggle[(int)ToggleIndices.Float]);
        _argsToggle[(int)ToggleIndices.Bool]= EditorGUILayout.ToggleLeft("bool", _argsToggle[(int)ToggleIndices.Bool]);
        _argsToggle[(int)ToggleIndices.String]=EditorGUILayout.ToggleLeft("string", _argsToggle[(int)ToggleIndices.String]);
        _argsToggle[(int)ToggleIndices.Custom] = EditorGUILayout.ToggleLeft("Custom args", _argsToggle[(int)ToggleIndices.Custom]);
        
        if (_argsToggle[(int)ToggleIndices.Custom])
        {
            customTypes = EditorGUILayout.TextField("Arguments (T1,T2,...TN):", customTypes);
        }
    }
    
    private void RenderOutputFilesSection()
    {
        if(_useTypeAsName && !_argsToggle[(int)ToggleIndices.Custom])
        {
            _eventName = _currentActiveToggle.ToString();
        }
        
        _eventSOName = string.Format(EventSONamePattern, _eventName);
        _eventListenerName = string.Format(EventListenerNamePattern, _eventName);
        
        EditorGUILayout.HelpBox($"Output files: \n\n" +
                                $"- {_eventSOName}.cs\n" +
                                $"- {_eventListenerName}.cs", MessageType.None);
    }
    
    private void RenderCreateAssetsButton()
    {
        if (GUILayout.Button("Create SO event scripts"))
        {
            string[] args = null;

            if(_currentActiveToggle != ToggleIndices.Custom 
               && _currentActiveToggle != ToggleIndices.NoArgs)
            {
                args = new string[1];
                args[0] = _currentActiveToggle.ToString().ToLower();
            }
            else if(_currentActiveToggle == ToggleIndices.Custom)
            {
                Debug.Log($"Custom types: " + customTypes);
                args = customTypes.Split(',');
            }

            AssetCreationMenu.CreateSOEventScripts(_eventSOName, _eventListenerName, args);

            Close();
        }
    }

    private void UpdateToggles()
    {
        for(int i=0; i < _argsToggle.Length; i++)
        {
            if (_argsToggle[i] != _argsTogglePreviousState[i] && _argsToggle[i]) //Toggle changed from false to true
            {
                _currentActiveToggle = (ToggleIndices)i;
                ClearTogglesExceptIndex(i);
                break;
            }
            else if(_argsToggle[i] != _argsTogglePreviousState[i] && !_argsToggle[i]) //toggle changed from true to false
            {
                _argsToggle[i] = true;
            }
        }

        //Update previous status:
        for (int i = 0; i < _argsToggle.Length; i++)
        {
            _argsTogglePreviousState[i] = _argsToggle[i];
        }
    }

    private void ClearTogglesExceptIndex(int skipIndex)
    {
        for (int i = 0; i < _argsToggle.Length; i++)
        {
            if(i!= skipIndex)
            {
                _argsToggle[i] = false;
            }
        }
    }
}
