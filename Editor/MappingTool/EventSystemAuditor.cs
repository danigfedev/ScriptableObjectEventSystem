using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using SOBaseEvents;

public class EventSystemAuditor : EditorWindow
{
    private Vector2 _scrollPos;
    private List<ScriptableObject> _allProjectEvents = new List<ScriptableObject>();
    
    // State management for UI expansion
    private Dictionary<ScriptableObject, bool> _expansionStates = new Dictionary<ScriptableObject, bool>();
    
    // Master data: Event -> Asset Path -> List of specific usages
    private Dictionary<ScriptableObject, Dictionary<string, List<UsageDetail>>> _masterResults = 
        new Dictionary<ScriptableObject, Dictionary<string, List<UsageDetail>>>();

    private struct UsageDetail
    {
        public string GameObjectName;
        public string ComponentTypeName;
        public Object Context; // Reference to ping the specific component
    }

    [MenuItem("Tools/SO Event System Auditor")]
    public static void ShowWindow() => GetWindow<EventSystemAuditor>("Event Auditor");

    private void OnEnable() => RefreshAndScanAll();

    private void OnGUI()
    {
        // --- TOP TOOLBAR ---
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Refresh & Scan Project", EditorStyles.toolbarButton)) RefreshAndScanAll();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Expand All", EditorStyles.toolbarButton)) SetAllExpansion(true);
        if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton)) SetAllExpansion(false);
        EditorGUILayout.EndHorizontal();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        if (_allProjectEvents.Count == 0)
        {
            EditorGUILayout.HelpBox("No ScriptableObjects implementing ISOEventBase were found in the project.", MessageType.Info);
        }

        foreach (var ev in _allProjectEvents)
        {
            RenderEventGroup(ev);
        }

        EditorGUILayout.EndScrollView();
    }

    private void RenderEventGroup(ScriptableObject ev)
    {
        // Default to expanded if state not found
        if (!_expansionStates.ContainsKey(ev)) _expansionStates[ev] = true;
        bool expanded = _expansionStates[ev];

        // Custom Header Style
        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniButtonMid);
        headerStyle.alignment = TextAnchor.MiddleLeft;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 11;
        headerStyle.fixedHeight = 25;

        // Visual Feedback: Highlight background if expanded
        if (expanded) GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); 

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Toggle Button
        string arrow = expanded ? "▼" : "▶";
        if (GUILayout.Button($" {arrow}  {ev.name.ToUpper()} [{ev.GetType().Name}]", headerStyle))
        {
            _expansionStates[ev] = !expanded;
        }
        GUI.backgroundColor = Color.white;

        if (expanded)
        {
            EditorGUILayout.Space(2);
            if (_masterResults.ContainsKey(ev) && _masterResults[ev].Count > 0)
            {
                foreach (var assetEntry in _masterResults[ev])
                {
                    // Group by Asset (Prefab/Scene)
                    EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    GUILayout.Label($"📂 {System.IO.Path.GetFileName(assetEntry.Key)}", EditorStyles.boldLabel);
                    
                    foreach (var detail in assetEntry.Value)
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

    private void SetAllExpansion(bool state)
    {
        var keys = _expansionStates.Keys.ToList();
        foreach (var key in keys) _expansionStates[key] = state;
    }

    // --- SCAN LOGIC ---

    private void RefreshAndScanAll()
    {
        _allProjectEvents.Clear();
        _masterResults.Clear();

        // 1. Find all relevant SOs in project
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so is ISOEventBase)
            {
                _allProjectEvents.Add(so);
                _masterResults[so] = new Dictionary<string, List<UsageDetail>>();
                if (!_expansionStates.ContainsKey(so)) _expansionStates[so] = true;
            }
        }

        // 2. Perform deep dependency scan
        string[] potentialAssets = AssetDatabase.FindAssets("t:Prefab t:Scene");
        foreach (var guid in potentialAssets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string[] deps = AssetDatabase.GetDependencies(assetPath);

            foreach (var ev in _allProjectEvents)
            {
                if (deps.Contains(AssetDatabase.GetAssetPath(ev)))
                {
                    if (assetPath.EndsWith(".prefab")) ScanPrefab(assetPath, ev);
                    else if (assetPath.EndsWith(".unity")) ScanScene(assetPath, ev);
                }
            }
        }
    }

    private void ScanPrefab(string path, ScriptableObject target)
    {
        GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        CheckComponents(path, root.GetComponentsInChildren<Component>(true), target);
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
        foreach (var comp in components)
        {
            if (comp == null) continue;
            
            // Iterate through all serialized properties to find the SO reference
            SerializedObject so = new SerializedObject(comp);
            SerializedProperty prop = so.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == target)
                {
                    if (!_masterResults[target].ContainsKey(assetPath)) 
                        _masterResults[target][assetPath] = new List<UsageDetail>();
                    
                    _masterResults[target][assetPath].Add(new UsageDetail {
                        GameObjectName = comp.gameObject.name,
                        ComponentTypeName = comp.GetType().Name,
                        Context = comp
                    });
                    break;
                }
            }
        }
    }
}