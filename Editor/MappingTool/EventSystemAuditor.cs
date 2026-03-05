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
    private ScriptableObject _selectedEvent;
    
    // Diccionario para guardar: Ruta del Asset -> Lista de detalles (GameObject + Componente)
    private Dictionary<string, List<UsageDetail>> _usageResults = new Dictionary<string, List<UsageDetail>>();

    private struct UsageDetail
    {
        public string GameObjectName;
        public string ComponentTypeName;
        public Object Context; // Para hacer ping al componente exacto
    }
    
    [MenuItem("EspidiGames/SO Events/SO Event System Auditor")]
    public static void ShowWindow() => GetWindow<EventSystemAuditor>("Event Auditor");

    private void OnEnable() => RefreshEventList();

    private void OnGUI()
    {
        RenderEventSelector();
        EditorGUILayout.Space(10);
        RenderUsageResults();
    }

    private void RenderEventSelector()
    {
        GUILayout.Label("1. Selección de Evento (ISOEventBase)", EditorStyles.boldLabel);
        if (GUILayout.Button("Refrescar Lista de Proyecto")) RefreshEventList();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(150));
        foreach (var ev in _allProjectEvents)
        {
            bool isSelected = (_selectedEvent == ev);
            GUI.color = isSelected ? Color.cyan : Color.white;
            if (GUILayout.Button($"{ev.name} ({ev.GetType().Name})", EditorStyles.miniButton))
            {
                _selectedEvent = ev;
                PerformDeepScan(ev);
            }
        }
        GUI.color = Color.white;
        EditorGUILayout.EndScrollView();
    }

    private void RenderUsageResults()
    {
        GUILayout.Label("2. Detalle de Referencias Encontradas", EditorStyles.boldLabel);
        if (_selectedEvent == null) return;

        if (_usageResults.Count == 0)
        {
            EditorGUILayout.HelpBox("No se han encontrado referencias directas en componentes.", MessageType.Info);
            return;
        }

        foreach (var entry in _usageResults)
        {
            EditorGUILayout.BeginVertical("helpbox");
            
            // Título: Nombre del Prefab o Escena
            GUILayout.Label(System.IO.Path.GetFileName(entry.Key), EditorStyles.whiteLargeLabel);
            
            foreach (var detail in entry.Value)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (GUILayout.Button($"GO: {detail.GameObjectName} | Comp: {detail.ComponentTypeName}", EditorStyles.label))
                {
                    EditorGUIUtility.PingObject(detail.Context);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }
    }

    private void RefreshEventList()
    {
        _allProjectEvents.Clear();
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so is ISOEventBase) _allProjectEvents.Add(so);
        }
    }

    private void PerformDeepScan(ScriptableObject targetEvent)
    {
        _usageResults.Clear();
        string targetPath = AssetDatabase.GetAssetPath(targetEvent);
        string[] potentialAssets = AssetDatabase.FindAssets("t:Prefab t:Scene");

        foreach (var guid in potentialAssets)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string[] deps = AssetDatabase.GetDependencies(path);
            if (!deps.Contains(targetPath)) continue;

            if (path.EndsWith(".prefab"))
            {
                ScanPrefab(path, targetEvent);
            }
            else if (path.EndsWith(".unity"))
            {
                ScanScene(path, targetEvent);
            }
        }
    }

    private void ScanPrefab(string path, ScriptableObject target)
    {
        GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var components = root.GetComponentsInChildren<Component>(true);
        CheckComponents(path, components, target);
    }

    private void ScanScene(string path, ScriptableObject target)
    {
        // Nota: Para escenas no abiertas, hay que cargarlas en el editor de forma temporal y silenciosa
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        var tempScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        
        var allComponents = Resources.FindObjectsOfTypeAll<Component>()
            .Where(c => c.gameObject.scene == tempScene);
        
        CheckComponents(path, allComponents, target);
        
        EditorSceneManager.CloseScene(tempScene, true);
    }

    private void CheckComponents(string assetPath, IEnumerable<Component> components, ScriptableObject target)
    {
        foreach (var comp in components)
        {
            if (comp == null) continue;

            // Usamos SerializedObject para iterar por todas las propiedades del componente
            // Esto detecta el evento incluso si está en un script personalizado que no sea un EventListener
            SerializedObject so = new SerializedObject(comp);
            SerializedProperty prop = so.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == target)
                {
                    if (!_usageResults.ContainsKey(assetPath)) _usageResults[assetPath] = new List<UsageDetail>();
                    
                    _usageResults[assetPath].Add(new UsageDetail {
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