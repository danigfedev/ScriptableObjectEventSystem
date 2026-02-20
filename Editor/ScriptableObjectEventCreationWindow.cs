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

    private bool _useTypeAsName = false;
    private bool[] _argsToggle = new bool[6];
    private bool[] _argsTogglePreviousState = new bool[6];
    private ToggleIndices currentActiveToggle;

    string eventName = "";
    string customTypes = "";

    //[MenuItem("Assets/Create/EspidiGames/SO Events/So Event Creation Window TEST", false, 0)]
    public static void OpenWindow()
    {
        //Calling this opens window normally (non-modal)
        var window = GetWindow<ScriptableObjectEventCreationWindow>(true, "Select Randomized Selected Objects");

        var windowDimensions = new Vector2(300, 400);
        window.minSize = windowDimensions;
        window.maxSize = windowDimensions;

        window._argsToggle[(int)ToggleIndices.NoArgs] = true;
        window._argsTogglePreviousState[(int)ToggleIndices.NoArgs] = true;
        window.currentActiveToggle = ToggleIndices.NoArgs;

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
        _useTypeAsName = EditorGUILayout.ToggleLeft("Use argument Type as name", _useTypeAsName);
        if (_argsToggle[(int)ToggleIndices.Custom])
        {
            EditorGUILayout.HelpBox($"Custom type is selected. It is recommended to add a custom name manually.", MessageType.Warning);
        }
        eventName = EditorGUILayout.TextField("Event name:", eventName);

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Event Arguments");
        EditorGUILayout.Space(5);

        //Toggle group behaviour: only one active simultaneously
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

        UpdateToggles();

        EditorGUILayout.Space(20);

        //Showing a preview of the scripts that will be generated (script names + extension (.cs)
        if(_useTypeAsName && !_argsToggle[(int)ToggleIndices.Custom])
        {
            eventName = currentActiveToggle.ToString();
        }
        
        string eventSOname = "SO" + eventName + "Event";
        string eventListenername = "SO" + eventName + "EventListener";
        EditorGUILayout.HelpBox($"Output files: - \n{eventSOname}.cs\n - {eventListenername}.cs", MessageType.None);

        //Creation button:
        //EditorGUILayout.Space(20);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create SO event scripts"))
        {
            string[] args = null;

            if(currentActiveToggle != ToggleIndices.Custom 
                && currentActiveToggle != ToggleIndices.NoArgs)
            {
                args = new string[1];
                args[0] = currentActiveToggle.ToString().ToLower();
            }
            else if(currentActiveToggle == ToggleIndices.Custom)
            {
                Debug.Log($"Custom types: "+customTypes);
                args = customTypes.Split(',');
            }

            AssetCreationMenu.CreateSOEventScripts(eventSOname, eventListenername, args);

            Close();
        }
            
    }
    
    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void UpdateToggles()
    {
        for(int i=0; i < _argsToggle.Length; i++)
        {
            if (_argsToggle[i] != _argsTogglePreviousState[i] && _argsToggle[i]) //Toggle changed from false to true
            {
                currentActiveToggle = (ToggleIndices)i;
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
