using UnityEngine;
using UnityEditor;

namespace LOK1game.Editor
{
    [CustomEditor(typeof(CharacterSpawnPoint))]
    public class CharacterSpawnPointEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var spawnPoint = (CharacterSpawnPoint)target;

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Ground Point"))
            {
                GroundSpawnPoint(spawnPoint);
            }
        }

        private void GroundSpawnPoint(CharacterSpawnPoint spawnPoint)
        {
            var ray = new Ray(spawnPoint.transform.position + Vector3.up, Vector3.down);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                Undo.RecordObject(spawnPoint.transform, "Ground Spawn Point");
                spawnPoint.transform.position = hit.point;
                EditorUtility.SetDirty(spawnPoint);
            }
        }
    }
}
