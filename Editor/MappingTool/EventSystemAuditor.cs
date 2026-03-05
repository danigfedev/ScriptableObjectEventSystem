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
    private Dictionary<ScriptableObject, bool> _expansionStates = new Dictionary<ScriptableObject, bool>();
    private Dictionary<ScriptableObject, Dictionary<string, List<UsageDetail>>> _masterResults = new Dictionary<ScriptableObject, Dictionary<string, List<UsageDetail>>>();

    private struct UsageDetail
    {
        public string GameObjectName;
        public string ComponentTypeName;
        public Object Context;
    }

    [MenuItem("Tools/SO Event System Auditor")]
    public static void ShowWindow() => GetWindow<EventSystemAuditor>("Event Auditor");

    private void OnEnable() => RefreshAndScanAll();

    private void OnGUI()
    {
        // --- TOOLBAR SUPERIOR ---
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Refrescar y Escanear Proyecto", EditorStyles.toolbarButton)) RefreshAndScanAll();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Expandir Todo", EditorStyles.toolbarButton)) SetAllExpansion(true);
        if (GUILayout.Button("Colapsar Todo", EditorStyles.toolbarButton)) SetAllExpansion(false);
        EditorGUILayout.EndHorizontal();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        if (_allProjectEvents.Count == 0)
        {
            EditorGUILayout.HelpBox("No se encontraron eventos que implementen ISOEventBase.", MessageType.Info);
        }

        foreach (var ev in _allProjectEvents)
        {
            RenderEventGroup(ev);
        }

        EditorGUILayout.EndScrollView();
    }

    private void RenderEventGroup(ScriptableObject ev)
    {
        if (!_expansionStates.ContainsKey(ev)) _expansionStates[ev] = true;
        bool expanded = _expansionStates[ev];

        // Definimos un estilo que cambie visualmente si está expandido
        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniButtonMid);
        headerStyle.alignment = TextAnchor.MiddleLeft;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 11;
        headerStyle.fixedHeight = 25;

        // Feedback visual: Si está expandido, resaltamos el botón
        if (expanded) GUI.backgroundColor = new Color(0.8f, 0.9f, 1f); 

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // El icono de flecha (foldout) ayuda a entender que es colapsable
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
                    // Contenedor para cada Prefab/Escena
                    EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    GUILayout.Label($"📂 {System.IO.Path.GetFileName(assetEntry.Key)}", EditorStyles.boldLabel);
                    
                    foreach (var detail in assetEntry.Value)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        // El botón de cada componente para hacer ping
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
                EditorGUILayout.LabelField("   No se detectaron usos en el proyecto.", EditorStyles.centeredGreyMiniLabel);
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

    // --- LÓGICA DE ESCANEO (Sin cambios significativos para mantener la funcionalidad) ---
    private void RefreshAndScanAll()
    {
        _allProjectEvents.Clear();
        _masterResults.Clear();

        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so is ISOEventBase) //
            {
                _allProjectEvents.Add(so);
                _masterResults[so] = new Dictionary<string, List<UsageDetail>>();
                if (!_expansionStates.ContainsKey(so)) _expansionStates[so] = true; // Expandido por defecto
            }
        }

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