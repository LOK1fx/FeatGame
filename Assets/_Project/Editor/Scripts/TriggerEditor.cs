using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using System.IO;

namespace LOK1game.Editor
{
    [CustomEditor(typeof(Trigger))]
    public class TriggerEditor : UnityEditor.Editor
    {
        private Trigger _trigger;

        private void OnEnable()
        {
            _trigger = (Trigger)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Visual Script"))
                CreateVisualScript();
        }

        private void CreateVisualScript()
        {
            var currentScene = EditorSceneManager.GetActiveScene();
            var sceneName = currentScene.name;
            var initialPath = $"Assets/_Game/Levels/{sceneName}";
            
            if (!Directory.Exists(initialPath))
            {
                Directory.CreateDirectory(initialPath);
                AssetDatabase.Refresh();
            }
            
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Visual Script",
                "TriggerScript",
                "asset",
                "Please enter a file name to save the visual script to",
                initialPath);

            if (string.IsNullOrEmpty(path))
                return;

            var graph = new FlowGraph();
            var scriptGraphAsset = ScriptableObject.CreateInstance<ScriptGraphAsset>();

            scriptGraphAsset.graph = graph;

            AssetDatabase.CreateAsset(scriptGraphAsset, path);
            AssetDatabase.SaveAssets();

            var scriptMachine = _trigger.gameObject.AddComponent<ScriptMachine>();
            scriptMachine.nest.source = GraphSource.Macro;
            scriptMachine.nest.macro = scriptGraphAsset;

            EditorUtility.SetDirty(_trigger);
            EditorUtility.SetDirty(scriptMachine);
            AssetDatabase.Refresh();
        }
    }
} 