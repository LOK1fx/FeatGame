using UnityEditor;
using UnityEngine;

namespace LOK1game.Editor
{
    [InitializeOnLoad]
    public static class UniqueActorIdInitializer
    {
        static UniqueActorIdInitializer()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            if (Application.isPlaying) return;

            var uniqueActorIds = Object.FindObjectsOfType<UniqueActorId>();
            foreach (var uniqueActorId in uniqueActorIds)
            {
                if (string.IsNullOrEmpty(uniqueActorId.Guid))
                {
                    var serializedObject = new SerializedObject(uniqueActorId);
                    var guidProperty = serializedObject.FindProperty("_guid");
                    
                    guidProperty.stringValue = System.Guid.NewGuid().ToString();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }

    [CustomEditor(typeof(UniqueActorId))]
    public class UniqueActorIdEditor : UnityEditor.Editor
    {
        private GUIStyle _largeLabelStyle;
        private GUIStyle _warningLabelStyle;

        private void InitializeStyles()
        {
            if (_largeLabelStyle == null)
            {
                _largeLabelStyle = new GUIStyle(EditorStyles.label);
                _largeLabelStyle.fontSize = 14;
                _largeLabelStyle.fontStyle = FontStyle.Bold;
            }

            if (_warningLabelStyle == null)
            {
                _warningLabelStyle = new GUIStyle(EditorStyles.label);
                _warningLabelStyle.fontSize = 12;
                _warningLabelStyle.fontStyle = FontStyle.Italic;
                _warningLabelStyle.normal.textColor = Color.yellow;
            }
        }

        public override void OnInspectorGUI()
        {
            InitializeStyles();
            var uniqueActorId = (UniqueActorId)target;
            var originalColor = GUI.color;

            if (PrefabUtility.IsPartOfPrefabAsset(uniqueActorId))
            {
                EditorGUILayout.LabelField("GUID can only be generated in scene or at runtime", _warningLabelStyle);
                return;
            }
            
            if (string.IsNullOrEmpty(uniqueActorId.Guid))
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("No GUID! Please generate a new one!", _largeLabelStyle);
                GUI.color = originalColor;
                DrawGenerateNewIdButton();
            }
            else
            {
                EditorGUILayout.LabelField("Current GUID:", _largeLabelStyle);
                EditorGUILayout.LabelField(uniqueActorId.Guid, _largeLabelStyle);
                DrawGenerateNewIdButton();

                if (GUILayout.Button("Clear GUID"))
                {
                    var serializedObject = new SerializedObject(target);
                    var guidProperty = serializedObject.FindProperty("_guid");
                    
                    guidProperty.stringValue = string.Empty;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawGenerateNewIdButton()
        {
            if (GUILayout.Button("Generate New ID"))
            {
                var serializedObject = new SerializedObject(target);
                var guidProperty = serializedObject.FindProperty("_guid");
                
                guidProperty.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
} 