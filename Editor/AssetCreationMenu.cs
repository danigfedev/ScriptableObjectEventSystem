using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using System.Text;
using System.Collections;
using Unity.EditorCoroutines.Editor;

namespace EG.ScriptableObjectSystem.Editor
{
    public class AssetCreationMenu : UnityEditor.Editor
    {
        private const string PackageRelativePathPattern = "Packages/{0}/";

        private const string EventIconRelativepath = "/Icons/event.png";
        private const string EventListenerIconRelativePath = "/Icons/listener.png";

        private const string GenericSOEventTemplatePath = "/CustomScriptTemplates/GenericSOEventTemplate.txt";
        private const string NoArgsSOEventTemplatePath = "/CustomScriptTemplates/NoArgsSOEventTemplate.txt";

        private const string GenericSOEventListenerTemplatePath = "/CustomScriptTemplates/GenericSOEventListenerTemplate.txt";
        private const string NoArgsSOEventListenerTemplatePath = "/CustomScriptTemplates/NoArgsSOEventListenerTemplate.txt";
        
        private static string _packageRelativePath;
        private static UnityEditor.PackageManager.PackageInfo _packageInfo;
        
        private class EditorAssetIconReplacer : UnityEditor.Editor
        {
            public IEnumerator AddIcon(string scriptPath, string iconName)
            {
                AssetDatabase.Refresh();

                yield return null; //Wait just for one editor frame
                
                var monoImporter = AssetImporter.GetAtPath(scriptPath) as MonoImporter;
                var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(_packageRelativePath + iconName);
                monoImporter.SetIcon(icon);
                monoImporter.SaveAndReimport();
            }
        }

        [MenuItem("Assets/Create/EspidiGames/SO Events/So Event Creation Window", false, 0)]
        static void OpenScriptableObjectEventCreationWindow()
        {
            SetPackageData();
            ScriptableObjectEventCreationWindow.OpenWindow();
        }

        public static void CreateSOEventScripts(string eventSOName, string eventListenerName, ArgInfo[] args)
        {
            //Picking the first one by default.
            //This will return the selected items' path, or the directory path, if no assets are selected
            var selectionGUID= Selection.assetGUIDs[0];
            var creationPath = AssetDatabase.GUIDToAssetPath(selectionGUID);

            //Checking whether the selection is a file or directory
            var fileAttributes = File.GetAttributes(creationPath);

            var isDirectory = (fileAttributes & FileAttributes.Directory) == FileAttributes.Directory;

            if (!isDirectory)
            {
                var fileInfo = new FileInfo(creationPath);
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

            var eventArgs = GenerateEventArguments(args);

            CreateSOEventScript(creationPath, eventSOName, eventListenerName, eventArgs);
            CreateSOEventListenerScript(creationPath, eventSOName, eventListenerName, eventArgs);

            //Refresh the Asset Database
            AssetDatabase.Refresh();
        }

        private static void CreateSOEventScript(string creationPath, string eventName, string listenerName, string[] argTypes)
        {
            //1-Load template asset
            TextAsset soEventTemplate;
            if (argTypes!= null)
            {
                soEventTemplate = AssetDatabase.LoadAssetAtPath(_packageRelativePath 
                    + GenericSOEventTemplatePath, typeof(TextAsset)) as TextAsset;
            }
            else
            {
                soEventTemplate = AssetDatabase.LoadAssetAtPath(_packageRelativePath 
                    + NoArgsSOEventTemplatePath, typeof(TextAsset)) as TextAsset;
            }

            //2-Check loaded object validity. If not valid, abort execution
            Assert.IsTrue(soEventTemplate != null, "[AssetCreation] SOEventTemplate loading failed. Aborting");

            //3-Complete template replacing placeholders with corresponding data
            var contents = soEventTemplate.text;

            contents = contents.Replace("<SCRIPT_NAME>", eventName);
            contents = contents.Replace("<SO_FILE_NAME>", eventName);
            contents = contents.Replace("<SO_MENU_NAME>", eventName);
            //Event order?
            contents = contents.Replace("<LISTENER_NAME>", listenerName);
            if(argTypes != null){
                contents = contents.Replace("<ARGUMENT_LIST_DEFINITION>", argTypes[0]);
                contents = contents.Replace("<ARGUMENT_LIST>", argTypes[1]);
                contents = contents.Replace("<CUSTOM_NAMESPACE_LIST>", argTypes[3]);
            }

            //4-Create file
            var filePath = creationPath + Path.DirectorySeparatorChar + eventName + ".cs";
            using (var sw = new StreamWriter(string.Format(filePath)))
            {
                sw.Write(contents);
            }

            //5-Add Custom Icon:
            var filePathInProject = GetPathInProjectAssets(filePath);
            var iconClass = new EditorAssetIconReplacer();
            EditorCoroutineUtility.StartCoroutineOwnerless(iconClass.AddIcon(filePathInProject, EventIconRelativepath));
        }

        private static void CreateSOEventListenerScript(string creationPath, string eventName, string listenerName, string[] argTypes)
        {
            //1-Load template asset
            TextAsset soEventListenerTemplate;
            if (argTypes != null)
            {
                soEventListenerTemplate = AssetDatabase.LoadAssetAtPath(_packageRelativePath 
                    + GenericSOEventListenerTemplatePath, typeof(TextAsset)) as TextAsset;

            }
            else
            {
                soEventListenerTemplate = AssetDatabase.LoadAssetAtPath(_packageRelativePath 
                    + NoArgsSOEventListenerTemplatePath, typeof(TextAsset)) as TextAsset;
            }

            //2-Check loaded object validity. If not valid, abort execution
            Assert.IsTrue(soEventListenerTemplate != null, "[AssetCreation] SOEventTemplate loading failed. Aborting");

            //3-Complete template replacing placeholders with corresponding data
            var contents = soEventListenerTemplate.text;

            contents = contents.Replace("<SCRIPT_NAME>", listenerName);
            contents = contents.Replace("<SO_EVENT_NAME>", eventName);
            contents = contents.Replace("<SO_EVENT_FIELD_NAME>", eventName.Replace("SO", "so"));
            if (argTypes != null)
            {
                contents = contents.Replace("<ARGUMENT_LIST_DEFINITION>", argTypes[0]);
                contents = contents.Replace("<ARGUMENT_LIST>", argTypes[1]);
                contents = contents.Replace("<ARGUMENT_TYPE_LIST>", argTypes[2]);
                contents = contents.Replace("<CUSTOM_NAMESPACE_LIST>", argTypes[3]);
            }

            //4-Create file
            var filePath = creationPath + Path.DirectorySeparatorChar + listenerName + ".cs";
            using (var sw = new StreamWriter(string.Format(filePath)))
            {
                sw.Write(contents);
            }

            //5-Add Custom Icon:
            var filePathInProject = GetPathInProjectAssets(filePath);
            var iconClass = new EditorAssetIconReplacer();
            EditorCoroutineUtility.StartCoroutineOwnerless(iconClass.AddIcon(filePathInProject, EventListenerIconRelativePath));
        }

        private static string[] GenerateEventArguments(ArgInfo[] argsList)
        {
            var result = new string[4];

            if (argsList == null || argsList.Length == 0)
            {
                return null;
            }

            var sb_definitions = new StringBuilder();
            var sb_argList = new StringBuilder();
            var sb_typeList = new StringBuilder();
            var sb_namespaceList = new StringBuilder();
            
            var argCount = 1;
            
            foreach (var argInfo in argsList)
            {
                //arg definitions
                sb_definitions.Append(argInfo.ArgType);
                sb_definitions.Append(" arg");
                sb_definitions.Append(argCount);
                
                //arg list:
                sb_argList.Append("arg");
                sb_argList.Append(argCount);
                
                //type list:
                sb_typeList.Append(argInfo.ArgType);
                
                //namespace list
                if (argInfo.SupportedType == EventSupportedArgs.Custom 
                    && !string.IsNullOrWhiteSpace(argInfo.ArgNamespace) 
                    && !sb_namespaceList.ToString().Contains(argInfo.ArgNamespace)) //avoid duplicates
                {
                    sb_namespaceList.AppendLine($"using {argInfo.ArgNamespace};");
                }
                
                if (argCount < argsList.Length)
                {
                    sb_definitions.Append(",");
                    sb_argList.Append(",");
                    sb_typeList.Append(",");
                }

                argCount++;
            }

            result[0] = sb_definitions.ToString(); //Type1 arg1, Type2 arg2, Type3 arg3...
            result[1] = sb_argList.ToString(); //arg1, arg2, arg3...
            result[2] = sb_typeList.ToString(); //Type1,Type2,Type3...
            result[3] = sb_namespaceList.ToString(); //using Custom.Namespace; ...

            return result;
        }
        
        private static string GetPathInProjectAssets(string fullPath)
        {
            const string AssetsFolder = "Assets";
            const string PackagesFolder = "Packages";
            
            var splitPath = fullPath.Split(Path.DirectorySeparatorChar);

            var sb = new StringBuilder();
            var flag = false;
            
            for (int i = 0; i < splitPath.Length; i++)
            {
                if (splitPath[i] == AssetsFolder || splitPath[i] == PackagesFolder)
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

            var path = sb.ToString();
            
            if (path.StartsWith(PackagesFolder))
            {
                path = path.Replace(_packageInfo.displayName, _packageInfo.name);
            }

            return path;
        }
        
        private static void SetPackageData()
        {
            var assembly = typeof(AssetCreationMenu).Assembly;
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            _packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            _packageRelativePath = string.Format(PackageRelativePathPattern, packageInfo.name);
        }
    }   
}