using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectEventCreationWindow : EditorWindow
{
    enum ToggleIndices
    {
        NoArgs = 0,
        Int,
        Float,
        Bool,
        String,
        Custom
    }

    bool useTypeAsName = false;
    bool[] argsToggle = new bool[6];
    bool[] argsToggle_prev = new bool[6];
    ToggleIndices currentActiveToggle;

    string eventName = "";
    string customTypes = "";

    //[MenuItem("Assets/Create/EspidiGames/SO Events/So Event Creation Window TEST", false, 0)]
    public static void OpenWindow()
    {
        //Calling this opens window normally (non-modal)
        ScriptableObjectEventCreationWindow window =
            EditorWindow.GetWindow<ScriptableObjectEventCreationWindow>(true, "Select Randomized Selected Objects");

        window.argsToggle[(int)ToggleIndices.NoArgs] = true;
        window.argsToggle_prev[(int)ToggleIndices.NoArgs] = true;
        window.currentActiveToggle = ToggleIndices.NoArgs;

        EditorCoroutineUtility.StartCoroutineOwnerless(window.MakeWindowModal());
    }

    //Coroutine that skips one editor frame and then makes Window modal.
    //Necessary to open window from Project panel (right click -> Create/EspidiGames/SO Events / Open window)
    //For whatever reason, opening from that context menu left the window completely blank
    //(it was working from unity menus though))
    public IEnumerator MakeWindowModal()
    {
        //Wait just for one editor frame
        yield return null;// new EditorWaitForSeconds(0.25f);

        this.ShowModalUtility();
    }

    void OnGUI()
    {
        useTypeAsName = EditorGUILayout.ToggleLeft("Use Type as name", useTypeAsName);
        if (argsToggle[(int)ToggleIndices.Custom])
        {
            EditorGUILayout.HelpBox($"Custom type is selected. It is recommended to add a custom name manually.", MessageType.Warning);
        }
        eventName = EditorGUILayout.TextField("Event name:", eventName);
        

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Event Arguments");
        EditorGUILayout.Space(5);

        //Toggle group behaviour: only one active simultaneously
        argsToggle[(int)ToggleIndices.NoArgs] = EditorGUILayout.ToggleLeft("No Argument", argsToggle[(int)ToggleIndices.NoArgs]);
        argsToggle[(int)ToggleIndices.Int] = EditorGUILayout.ToggleLeft("int", argsToggle[(int)ToggleIndices.Int]);
        argsToggle[(int)ToggleIndices.Float] = EditorGUILayout.ToggleLeft("float", argsToggle[(int)ToggleIndices.Float]);
        argsToggle[(int)ToggleIndices.Bool]= EditorGUILayout.ToggleLeft("bool", argsToggle[(int)ToggleIndices.Bool]);
        argsToggle[(int)ToggleIndices.String]=EditorGUILayout.ToggleLeft("string", argsToggle[(int)ToggleIndices.String]);
        argsToggle[(int)ToggleIndices.Custom] = EditorGUILayout.ToggleLeft("Custom args", argsToggle[(int)ToggleIndices.Custom]);
        
        if (argsToggle[(int)ToggleIndices.Custom])
        {
            customTypes = EditorGUILayout.TextField("Arguments (T1,T2,...TN):", customTypes);
        }

        UpdateToggles();

        EditorGUILayout.Space(20);

        //Showing a preview of the scripts that will be generated (script names + extension (.cs)
        if(useTypeAsName && !argsToggle[(int)ToggleIndices.Custom])
        {
            eventName = currentActiveToggle.ToString();
        }
        
        string eventSOname = "SO" + eventName + "Event";
        string eventListenername = "SO" + eventName + "EventListener";
        EditorGUILayout.HelpBox($"Help box \n{eventSOname}.cs\n{eventListenername}.cs", MessageType.None);

        //Creation button:
        EditorGUILayout.Space(20);

        if (GUILayout.Button("Create SO event scripts"))
        {
            string[] args = null;

            if(currentActiveToggle != ToggleIndices.Custom 
                && currentActiveToggle != ToggleIndices.NoArgs)
            {
                args = new string[1];
                args[0] = currentActiveToggle.ToString().ToLower(); ;
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

    private void UpdateToggles()
    {
        for(int i=0; i < argsToggle.Length; i++)
        {
            if (argsToggle[i] != argsToggle_prev[i] && argsToggle[i]) //Toggle changed from false to true
            {
                currentActiveToggle = (ToggleIndices)i;
                ClearTogglesExceptIndex(i);
                break;
            }
            else if(argsToggle[i] != argsToggle_prev[i] && !argsToggle[i]) //toggle changed from true to false
            {
                argsToggle[i] = true;
            }
        }

        //Update previous status:
        for (int i = 0; i < argsToggle.Length; i++)
        {
            argsToggle_prev[i] = argsToggle[i];
        }
    }

    private void ClearTogglesExceptIndex(int skipIndex)
    {
        for (int i = 0; i < argsToggle.Length; i++)
        {
            if(i!= skipIndex)
            {
                argsToggle[i] = false;
            }
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
