using System.Collections.Generic;
using System.Linq;
using SOBaseEvents;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor.AuditorTool
{
    public class EventSystemAuditor : EditorWindow
    {
        private const string PrefabExtension = ".prefab";
        private const string SceneExtension = ".unity";
    
        private Vector2 _scrollPos;
        private List<ScriptableObject> _allProjectEvents = new List<ScriptableObject>();
        private Dictionary<ScriptableObject, bool> _expandedStates = new Dictionary<ScriptableObject, bool>();
        private Dictionary<ScriptableObject, Dictionary<string, List<UsageDetail>>> _masterResults = 
            new Dictionary<ScriptableObject, Dictionary<string, List<UsageDetail>>>();
        private GUIStyle _headerStyle;

        private struct UsageDetail
        {
            public string GameObjectName;
            public string ComponentTypeName;
            public Object Context;
        }

        [MenuItem("EspidiGames/SO Events/SO Event System Auditor")]
        public static void ShowWindow()
        {
            GetWindow<EventSystemAuditor>("Event Auditor");
        }

        private void OnEnable()
        {
            GetWindow<EventSystemAuditor>("Event Auditor");
            RefreshAndScanAll();
        }

        private void OnGUI()
        {
            RenderTopToolBar();
            RenderResultsSection();
        }
    
        private void RenderTopToolBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
            if (GUILayout.Button("Refresh & Scan Project", EditorStyles.toolbarButton))
            {
                RefreshAndScanAll();
            }
        
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Expand All", EditorStyles.toolbarButton))
            {
                SetEventUsagesExpandedState(true);
            }

            if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton))
            {
                SetEventUsagesExpandedState(false);
            }
        
            EditorGUILayout.EndHorizontal();
        }

        private void RenderResultsSection()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_allProjectEvents.Count == 0)
            {
                EditorGUILayout.HelpBox("No ScriptableObjects implementing ISOEventBase were found in the project.",
                    MessageType.Info);
            }

            foreach (var soEvent in _allProjectEvents)
            {
                RenderSOEventsUsages(soEvent);
            }

            EditorGUILayout.EndScrollView();
        }
    
        private void RenderSOEventsUsages(ScriptableObject soEvent)
        {
            // Default to expanded if state not found
            if (!_expandedStates.ContainsKey(soEvent))
            {
                _expandedStates[soEvent] = true;
            }
        
            var expanded = _expandedStates[soEvent];
            _headerStyle ??= CreateHeaderStyle();

            // Visual Feedback: Highlight background if expanded
            if (expanded)
            {
                GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);
            } 

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
            RenderScriptableObjectEventEntryHeaderToggle(soEvent, expanded);
            GUI.backgroundColor = Color.white;

            if (expanded)
            {
                EditorGUILayout.Space(2);
                if (_masterResults.ContainsKey(soEvent) && _masterResults[soEvent].Count > 0)
                {
                    foreach (var assetEntry in _masterResults[soEvent])
                    {
                        // Group by Asset (Prefab/Scene)
                        EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    
                        GUILayout.Label($"📂 {System.IO.Path.GetFileName(assetEntry.Key)}", EditorStyles.boldLabel);
                    
                        foreach (var detail in assetEntry.Value)
                        {
                            RenderSOEventUsage(detail);
                        }
                    
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(1);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("   No usages detected in prefabs or scenes.", EditorStyles.centeredGreyMiniLabel);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private static GUIStyle CreateHeaderStyle()
        {
            var headerStyle = new GUIStyle(EditorStyles.miniButtonMid);
            headerStyle.alignment = TextAnchor.MiddleLeft;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = 11;
            headerStyle.fixedHeight = 25;

            return headerStyle;
        }
    
        private void RenderScriptableObjectEventEntryHeaderToggle(ScriptableObject soEvent, bool expanded)
        {
            var arrow = expanded ? "▼" : "▶";
            if (GUILayout.Button($" {arrow}  {soEvent.name.ToUpper()} [{soEvent.GetType().Name}]", _headerStyle))
            {
                _expandedStates[soEvent] = !expanded;
            }
        }
    
        private static void RenderSOEventUsage(UsageDetail detail)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            // Deep link to component
            if (GUILayout.Button($"   # GO: {detail.GameObjectName} ({detail.ComponentTypeName})", EditorStyles.label))
            {
                EditorGUIUtility.PingObject(detail.Context);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SetEventUsagesExpandedState(bool expand)
        {
            var keys = _expandedStates.Keys.ToList();
        
            foreach (var key in keys)
            {
                _expandedStates[key] = expand;
            }
        }

        #region === Asset scanning logic ===
    
        private void RefreshAndScanAll()
        {
            _allProjectEvents.Clear();
            _masterResults.Clear();
        
            FindAllScriptableObjectEventAssets();
            ScanDependencies();
        }

        private void FindAllScriptableObjectEventAssets()
        {
            var guids = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableObjectAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            
                if (scriptableObjectAsset is ISOEventBase)
                {
                    _allProjectEvents.Add(scriptableObjectAsset);
                    _masterResults[scriptableObjectAsset] = new Dictionary<string, List<UsageDetail>>();
                    if (!_expandedStates.ContainsKey(scriptableObjectAsset))
                    {
                        _expandedStates[scriptableObjectAsset] = true;
                    }
                }
            }
        }

        private void ScanDependencies()
        {
            var potentialAssets = AssetDatabase.FindAssets("t:Prefab t:Scene");
            foreach (var guid in potentialAssets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var dependencies = AssetDatabase.GetDependencies(assetPath);

                foreach (var soEvent in _allProjectEvents)
                {
                    if (dependencies.Contains(AssetDatabase.GetAssetPath(soEvent)))
                    {
                        if (assetPath.EndsWith(PrefabExtension))
                        {
                            ScanPrefab(assetPath, soEvent);
                        }
                        else if (assetPath.EndsWith(SceneExtension))
                        {
                            ScanScene(assetPath, soEvent);
                        }
                    }
                }
            }
        }
    
        private void ScanPrefab(string path, ScriptableObject target)
        {
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var allComponents = root.GetComponentsInChildren<Component>(true);
            CheckComponents(path, allComponents, target);
        }

        private void ScanScene(string path, ScriptableObject target)
        {
            // Load scene additively and silently to inspect contents
            var tempScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            var allComponents = Resources.FindObjectsOfTypeAll<Component>().Where(c => c.gameObject.scene == tempScene);
            CheckComponents(path, allComponents, target);
            EditorSceneManager.CloseScene(tempScene, true);
        }

        private void CheckComponents(string assetPath, IEnumerable<Component> components, ScriptableObject target)
        {
            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }
            
                // Iterate through all serialized properties to find the SO reference
                var serializedObject = new SerializedObject(component);
                var property = serializedObject.GetIterator();

                while (property.NextVisible(true))
                {
                    if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == target)
                    {
                        if (!_masterResults[target].ContainsKey(assetPath))
                        {
                            _masterResults[target][assetPath] = new List<UsageDetail>();
                        }
                    
                        _masterResults[target][assetPath].Add(new UsageDetail {
                            GameObjectName = component.gameObject.name,
                            ComponentTypeName = component.GetType().Name,
                            Context = component
                        });
                        break;
                    }
                }
            }
        }
    
        #endregion
    }
}