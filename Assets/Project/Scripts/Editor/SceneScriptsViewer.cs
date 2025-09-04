using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class SceneScriptsViewer : EditorWindow
{
    private Vector2 scrollPos;

    private List<Type> userScripts = new List<Type>();
    private List<Type> unityScripts = new List<Type>();
    private List<Type> uiScripts = new List<Type>();

    private bool showUserScripts = true;
    private bool showUnityScripts = true;
    private bool showUIScripts = true;

    [MenuItem("Tools/Scene Scripts Viewer")]
    public static void ShowWindow()
    {
        GetWindow<SceneScriptsViewer>("Scene Scripts Viewer");
    }

    private void OnEnable()
    {
        RefreshScripts();
    }

    private void OnHierarchyChange()
    {
        RefreshScripts();
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("Scripts in Current Scene", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // ==== User Scripts ====
        EditorGUILayout.BeginVertical("box");
        showUserScripts = EditorGUILayout.Foldout(showUserScripts, $"User Scripts ({userScripts.Count + uiScripts.Count})", true);
        if (showUserScripts)
        {
            // UI Scripts subgroup
            if (uiScripts.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                showUIScripts = EditorGUILayout.Foldout(showUIScripts, $"UI Scripts ({uiScripts.Count})", true);
                if (showUIScripts)
                {
                    foreach (var type in uiScripts.OrderBy(t => t.Name))
                        DrawScriptButton(type);
                }
                EditorGUILayout.EndVertical();
            }

            // Other User Scripts
            foreach (var type in userScripts.OrderBy(t => t.Name))
                DrawScriptButton(type);
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // ==== Unity Scripts ====
        EditorGUILayout.BeginVertical("box");
        showUnityScripts = EditorGUILayout.Foldout(showUnityScripts, $"Unity Scripts ({unityScripts.Count})", true);
        if (showUnityScripts)
        {
            foreach (var type in unityScripts.OrderBy(t => t.Name))
                DrawScriptButton(type);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Refresh"))
        {
            RefreshScripts();
        }
    }

    private void DrawScriptButton(Type type)
    {
        if (GUILayout.Button($"{type.Name} ({CountObjectsWithType(type)})"))
        {
            var objects = GameObject.FindObjectsOfType(type);
            Selection.objects = Array.ConvertAll(objects, obj => ((Component)obj).gameObject);
        }
    }

    private int CountObjectsWithType(Type type)
    {
        return GameObject.FindObjectsOfType(type).Length;
    }

    private void RefreshScripts()
    {
        userScripts.Clear();
        unityScripts.Clear();
        uiScripts.Clear();

        var allObjects = GameObject.FindObjectsOfType<MonoBehaviour>();
        HashSet<Type> typesSet = new HashSet<Type>();

        foreach (var mb in allObjects)
        {
            typesSet.Add(mb.GetType());
        }

        foreach (var type in typesSet)
        {
            string ns = type.Namespace ?? "";
            string name = type.Name;

            if (ns.StartsWith("UnityEngine") || ns.StartsWith("UnityEditor"))
            {
                unityScripts.Add(type);
            }
            else
            {
                if (name.StartsWith("UI"))
                    uiScripts.Add(type);
                else
                    userScripts.Add(type);
            }
        }
    }
}
