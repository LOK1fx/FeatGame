using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace LOK1game.Editor
{
    
    public class SceneManagerEditorWindow : EditorWindow
    {
        private List<EditorBuildSettingsScene> _loadedEditorScenes = new List<EditorBuildSettingsScene>();
        private List<Scene> _openedEditorScenes = new List<Scene>();

        private Vector2 _scenesScrollViewPosition;

        [MenuItem("LOK1game Tools/SceneManager")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneManagerEditorWindow>();
            window.titleContent = new GUIContent("LOK1game Scene Manager");
        }

        private void OnGUI()
        {
            _scenesScrollViewPosition = GUILayout.BeginScrollView(_scenesScrollViewPosition, true, true);
            UpdateScenesViewList();
            GUILayout.EndScrollView();

            if (GUILayout.Button("Load all list or refresh"))
            {
                _loadedEditorScenes = EditorBuildSettings.scenes.ToList();
            }
        }

        private string ConvertScenePathToName(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var lastSlashIndex = fileName.LastIndexOf('/');

            return fileName.Substring(lastSlashIndex + 1);
        }

        private void UpdateScenesViewList()
        {
            var normalContentColor = GUI.contentColor;

            foreach (var scene in _loadedEditorScenes)
            {
                if (GUILayout.Button(
                    new GUIContent($"{ConvertScenePathToName(scene.path)}",
                    $"path: {scene.path}")))
                {
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);

                    _openedEditorScenes.Clear();
                }

                GUILayout.BeginHorizontal();

                GUI.contentColor = Color.green;

                if (TryGetSceneByPath(scene.path, out var foundAdditiveScene) && foundAdditiveScene.isLoaded == false)
                {
                    if (GUILayout.Button("Additive"))
                        _openedEditorScenes.Add(EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive));
                }

                if (TryGetSceneByPath(scene.path, out var foundScene) && foundScene.isLoaded == true)
                {
                    GUI.contentColor = Color.red;

                    if (GUILayout.Button("Unload"))
                    {
                        if (_openedEditorScenes == null)
                            return;

                        EditorSceneManager.CloseScene(foundScene, true);
                    }
                }

                GUILayout.EndHorizontal();

                GUI.contentColor = normalContentColor;

                GUILayout.Space(4f);
            }
        }

        private bool TryGetSceneByPath(string path, out Scene foundScene)
        {
            foreach (var scene in _loadedEditorScenes)
            {
                foundScene = _openedEditorScenes.Where(s => s.path == path).FirstOrDefault();

                return true;
            }

            foundScene = new Scene();

            return false;
        }

        private GUIStyle GetColoredButtonBackground(Color color)
        {
            var buttonStyle = new GUIStyle(GUI.skin.button);

            buttonStyle.normal.background = MakeBackgroundTexture(1, 1, color);
            buttonStyle.margin = new RectOffset(4, 4, 2, 2);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = Color.black;

            return buttonStyle;
        }

        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            var backgroundTexture = new Texture2D(width, height);

            backgroundTexture.SetPixels(pixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }
    }
}