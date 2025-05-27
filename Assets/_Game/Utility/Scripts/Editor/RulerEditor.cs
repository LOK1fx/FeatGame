#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace LOK1game.Editor
{
    public class RulerEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Create Ruler", false, 0)]
        private static void CreateRuler()
        {
            var rulerObj = new GameObject("Ruler");
            var ruler = rulerObj.AddComponent<Ruler>();
            var createPointsMethod = typeof(Ruler).GetMethod("CreatePoints",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            createPointsMethod?.Invoke(ruler, null);

            Selection.activeGameObject = rulerObj;

            Undo.RegisterCreatedObjectUndo(rulerObj, "Create Ruler");
        }
    }
}

#endif