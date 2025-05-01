using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

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
            {
                CreateVisualScript();
            }
        }

        private void CreateVisualScript()
        {
            // Создаем путь для сохранения ассета
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Visual Script",
                "TriggerScript",
                "asset",
                "Please enter a file name to save the visual script to");

            if (string.IsNullOrEmpty(path))
                return;

            // Создаем новый граф
            var graph = new FlowGraph();
            
            // Создаем ScriptGraphAsset
            var scriptGraphAsset = ScriptableObject.CreateInstance<ScriptGraphAsset>();
            scriptGraphAsset.graph = graph;

            // Сохраняем ассет
            AssetDatabase.CreateAsset(scriptGraphAsset, path);
            AssetDatabase.SaveAssets();

            // Добавляем Script Machine на объект
            var scriptMachine = _trigger.gameObject.AddComponent<ScriptMachine>();
            scriptMachine.nest.source = GraphSource.Macro;
            scriptMachine.nest.macro = scriptGraphAsset;

            // Обновляем инспектор
            EditorUtility.SetDirty(_trigger);
            EditorUtility.SetDirty(scriptMachine);
            AssetDatabase.Refresh();
        }
    }
} 