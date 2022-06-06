using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System.Text;
using System.Collections;
using Unity.EditorCoroutines.Editor;

public class AssetCreationMenu : Editor
{
    //private const string packageRelativePath = "Assets/EspidiGames/ScriptableObjectEventSystem/";// Use this to test functionality if developing new features
    private const string packageRelativePath = "Packages/com.espidigames.scriptable-object-event-system/"; //Release path to work from packages directory

    private const string eventIconRelativepath = "/Icons/event.png";
    private const string eventListenerIconRelativePath = "/Icons/listener.png";

    private const string genericSOEventTemplatePath = "/CustomScriptTemplates/GenericSOEventTemplate.txt";
    private const string noArgsSOEventTemplatePath = "/CustomScriptTemplates/NoArgsSOEventTemplate.txt";

    private const string genericSOEventListenerTemplatePath = "/CustomScriptTemplates/GenericSOEventListenerTemplate.txt";
    private const string NoArgsSOEventListenerTemplatePath = "/CustomScriptTemplates/NoArgsSOEventListenerTemplate.txt";

    
        


    //Nested class that handles script's icon modification
    private class IconReplacementClass : Editor
    {
        public IEnumerator AddIcon(string scriptPath, string iconName)
        {
            //string scriptPath = "Assets/EspidiGamesScriptTemplates/" + scriptName;
            AssetDatabase.Refresh();

            //Wait just for one editor frame
            yield return new EditorWaitForSeconds(0.1f); // hardcoded waiting time

            var monoImporter = AssetImporter.GetAtPath(scriptPath) as MonoImporter;
            //var monoScript = monoImporter.GetScript();
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(packageRelativePath + iconName);
            monoImporter.SetIcon(icon);
            monoImporter.SaveAndReimport();
        }
    }

    [MenuItem("Assets/Create/EspidiGames/SO Events/So Event Creation Window", false, 0)]
    static void OpenScriptalbeObjectEventCreationWindow()
    {
        ScriptableObjectEventCreationWindow.OpenWindow();
    }

    public static void CreateSOEventScripts(string eventSOname, string eventListenername, string[] args)//pass parameters from window
    {
        //Picking the first one by default.
        //This will return the selected items' path, or the directoyr path, if no assets are selected
        string selectionGUID= Selection.assetGUIDs[0];
        string creationPath = AssetDatabase.GUIDToAssetPath(selectionGUID);

        //Checking whether the selection is a file or directory
        FileAttributes fileAttributes = File.GetAttributes(creationPath);

        bool isDirectory = (fileAttributes & FileAttributes.Directory) == FileAttributes.Directory;

        if (!isDirectory)
        {
            FileInfo fileInfo = new FileInfo(creationPath);
            creationPath = fileInfo.Directory.ToString(); //This returns the full path C://path to project/Assets/...
        }
        else
        {
            //Build full path
            creationPath = new FileInfo(creationPath).FullName;
        }

        //Right clicking without selections returns just the relative path from Assest/whatever... -> Application.dataPath

        //Asset creation sample:
        //Create file traditionally (TextAssets are readonly. Useful fro creating them in a third party tool and format, import them in Unity and then work with them.)

        //Loading the template text file which has some code already in it.
        //Note that the text file is stored in the path PROJECT_NAME/Assets/CharacterTemplate.txt

        string[] eventArgs = GenerateEventArguments(args);

        CreateSOEventScript(creationPath, eventSOname, eventListenername, eventArgs);
        CreateSOEventListenerScript(creationPath, eventSOname, eventListenername, eventArgs);

        //Refresh the Asset Database
        AssetDatabase.Refresh();
    }

    private static void CreateSOEventScript(string creationPath, string eventName, string listenerName, string[] argTypes)
    {
        //1-Load template asset
        TextAsset soEventTemplate;
        if (argTypes!= null)
        {
            soEventTemplate = AssetDatabase.LoadAssetAtPath(packageRelativePath 
                + genericSOEventTemplatePath, typeof(TextAsset)) as TextAsset;
        }
        else
        {
            soEventTemplate = AssetDatabase.LoadAssetAtPath(packageRelativePath 
                + noArgsSOEventTemplatePath, typeof(TextAsset)) as TextAsset;
        }

        //2-Check loaded object validity. If not valid, abort execution
        Assert.IsTrue(soEventTemplate != null, "[AssetCreation] SOEventTemplate loading failed. Aborting");

        //3-Complete template replacing placeholders with corresponding data
        string contents = soEventTemplate.text;

        contents = contents.Replace("<SCRIPT_NAME>", eventName);
        contents = contents.Replace("<SO_FILE_NAME>", eventName);
        contents = contents.Replace("<SO_MENU_NAME>", eventName);
        //Event order?
        contents = contents.Replace("<LISTENER_NAME>", listenerName);
        if(argTypes != null){
            contents = contents.Replace("<ARGUMENT_LIST_DEFINITION>", argTypes[0]);
            contents = contents.Replace("<ARGUMENT_LIST>", argTypes[1]);
        }

        //4-Create file
        string filePath = creationPath + Path.DirectorySeparatorChar + eventName + ".cs";
        using (StreamWriter sw = new StreamWriter(string.Format(filePath)))
        {
            sw.Write(contents);
        }

        //5-Add Custom Icon:
        string filePathInProject = GetPathInProjectAssets(filePath);
        var iconClass = new IconReplacementClass();
        EditorCoroutineUtility.StartCoroutineOwnerless(iconClass.AddIcon(filePathInProject, eventIconRelativepath));

    }
    

    private static void CreateSOEventListenerScript(string creationPath, string eventName, string listenerName, string[] argTypes)
    {
        //1-Load template asset
        TextAsset soEventListenerTemplate;
        if (argTypes != null)
        {
            soEventListenerTemplate = AssetDatabase.LoadAssetAtPath(packageRelativePath 
                + genericSOEventListenerTemplatePath, typeof(TextAsset)) as TextAsset;

        }
        else
        {
            soEventListenerTemplate = AssetDatabase.LoadAssetAtPath(packageRelativePath 
                + NoArgsSOEventListenerTemplatePath, typeof(TextAsset)) as TextAsset;
        }

        //2-Check loaded object validity. If not valid, abort execution
        Assert.IsTrue(soEventListenerTemplate != null, "[AssetCreation] SOEventTemplate loading failed. Aborting");

        //3-Complete template replacing placeholders with corresponding data
        string contents = soEventListenerTemplate.text;

        contents = contents.Replace("<SCRIPT_NAME>", listenerName);
        contents = contents.Replace("<SO_EVENT_NAME>", eventName);
        contents = contents.Replace("<SO_EVENT_FIELD_NAME>", eventName.Replace("SO", "so"));
        if (argTypes != null)
        {
            contents = contents.Replace("<ARGUMENT_LIST_DEFINITION>", argTypes[0]);
            contents = contents.Replace("<ARGUMENT_LIST>", argTypes[1]);
            contents = contents.Replace("<ARGUMENT_TYPE_LIST>", argTypes[2]);
        }

        //4-Create file
        string filePath = creationPath + Path.DirectorySeparatorChar + listenerName + ".cs";
        using (StreamWriter sw = new StreamWriter(string.Format(filePath)))
        //using (StreamWriter sw = new StreamWriter(string.Format(creationPath + Path.DirectorySeparatorChar + listenerName + ".cs")))
        {
            sw.Write(contents);
        }

        //5-Add Custom Icon:
        string filePathInProject = GetPathInProjectAssets(filePath);
        var iconClass = new IconReplacementClass();
        EditorCoroutineUtility.StartCoroutineOwnerless(iconClass.AddIcon(filePathInProject, eventListenerIconRelativePath));
    }

    private static string[] GenerateEventArguments(string[] argTypes)
    {
        string[] result = new string[3];

        if (argTypes == null)
        {
            return null;
        }

            StringBuilder sb_definitions = new StringBuilder();
        StringBuilder sb_argList = new StringBuilder();
        StringBuilder sb_typeList = new StringBuilder();
        int argCount = 1;
        foreach (string type in argTypes)
        {
            //arg definitions
            sb_definitions.Append(type);
            sb_definitions.Append(" arg");
            sb_definitions.Append(argCount);
            if (argCount < argTypes.Length)
            {
                sb_definitions.Append(",");
            }

            //arg list:
            sb_argList.Append("arg");
            sb_argList.Append(argCount);
            if (argCount < argTypes.Length)
            {
                sb_argList.Append(",");
            }

            //type list:
            sb_typeList.Append(type);
            if (argCount < argTypes.Length)
            {
                sb_typeList.Append(",");
            }

            argCount++;
        }

        result[0] = sb_definitions.ToString(); //Type1 arg1, Type2 arg2, Type3 arg3...
        result[1] = sb_argList.ToString(); //arg1, arg2, arg3...
        result[2] = sb_typeList.ToString(); //Type1,Type2,Type3...

        return result;
    }

    
    private static string GetPathInProjectAssets(string fullPath)
    {
        string[] splitPath = fullPath.Split(Path.DirectorySeparatorChar);

        StringBuilder sb = new StringBuilder();
        bool flag = false;
        for (int i = 0; i < splitPath.Length; i++)
        {
            if (splitPath[i] == "Assets")
            {
                flag = true;
            }

            if (flag)
            {
                sb.Append(splitPath[i]);
                if (i < splitPath.Length - 1)
                {
                    sb.Append(Path.DirectorySeparatorChar);
                }
            }
        }

        return sb.ToString();
    }

}