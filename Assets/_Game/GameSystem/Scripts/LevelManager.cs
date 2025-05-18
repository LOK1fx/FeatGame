using LOK1game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelManager
{
    public static List<LevelData> LevelsData { get; private set; }

    [SerializeField] private List<LevelData> _levelsData = new List<LevelData>();

    public void Initialize()
    {
        LevelsData = _levelsData;
    }

    public static void LoadLevel(LevelData data)
    {
        if (LevelsData.Contains(data) == false)
            throw new KeyNotFoundException("There is no that level data in level manager! Add it.");

        SceneManager.LoadSceneAsync(data.MainSceneName, LoadSceneMode.Single);

        foreach(var addativeScene in data.AdditiveScenes)
        {
            SceneManager.LoadSceneAsync(addativeScene, LoadSceneMode.Additive);
        }
    }

    [Obsolete]
    public static void LoadNextLevel()
    {
        var currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadSceneAsync(currentScene + 1);
    }

    public static void RestartLevel()
    {
        var currentLevel = GetCurrentLevelData();

        LoadLevel(currentLevel);
    }

    public static LevelData GetCurrentLevelData()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;

        foreach (var level in LevelsData)
        {
            if(level.MainSceneName == currentSceneName)
            {
                return level;
            }
        }

        Debug.LogError($"LevelManager can't find a level with the SceneName {currentSceneName}.");

        return null;
    }

    public static LevelData GetLevelData(string mainSceneName)
    {
        var levelData = LevelsData.Where(level => level.MainSceneName == mainSceneName).FirstOrDefault();

        return levelData;
    }
}
