using LOK1game;
using LOK1game.Tools;
using LOK1game.Utility;
using LOK1game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    #region LevelLoading

    public static IEnumerator LoadLevel(LevelData data)
    {
        GetLogger().Push($"LoadLevel started with data: {data?.DisplayName ?? "null"}");
        
        if (data == null)
        {
            GetLogger().PushError("LoadLevel failed: LevelData is null");
            yield break;
        }

        if (LevelsData.Contains(data) == false)
            throw new KeyNotFoundException("There is no that level data in level manager! Add it.");

        GetLogger().Push($"-> {data.DisplayName} ({data.MainSceneName})");

        // TODO: Show loading screen

        GetLogger().Push($"Starting to load main scene: {data.MainSceneName}");
        SceneManager.LoadSceneAsync(data.MainSceneName, LoadSceneMode.Single);
        SceneManager.activeSceneChanged += OnMainSceneLoaded;

        if (data.AdditiveScenes.Count > 0)
        {
            GetLogger().Push($"Loading {data.AdditiveScenes.Count} additive scenes");
            foreach (var addativeScene in data.AdditiveScenes)
            {
                SceneManager.LoadSceneAsync(addativeScene, LoadSceneMode.Additive);
            }
        }
        GetLogger().Push($"Additive scenes loaded ({data.AdditiveScenes.Count})");
    }

    private static void OnMainSceneLoaded(Scene previous, Scene newActive)
    {
        GetLogger().Push($"Scene is now active: {SceneManager.GetActiveScene().name}");

        Coroutines.StartRoutine(ApplyGameMode(GetLevelData(newActive.name)));

        GetLogger().Push($"Level = {newActive.name}");


        SceneManager.activeSceneChanged -= OnMainSceneLoaded;
    }

    private static IEnumerator ApplyGameMode(LevelData data)
    {
        yield return App.ProjectContext.GameModeManager.SwitchGameModeRoutine(data.LevelGameMode);

        // TODO: Hide loading screen
    }

    #endregion

    public static void RestartLevel()
    {
        var currentLevel = GetCurrentLevelData();
        
        if (currentLevel == null)
        {
            GetLogger().PushError("RestartLevel failed: currentLevel is null");
            return;
        }
        
        GetLogger().Push($"Restarting level: {currentLevel.DisplayName}");
        Coroutines.StartRoutine(LoadLevel(currentLevel));
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

        GetLogger().PushError($"LevelManager can't find a level with the SceneName {currentSceneName}.");

        return null;
    }

    public static LevelData GetLevelData(string mainSceneName)
    {
        var levelData = LevelsData.Where(level => level.MainSceneName == mainSceneName).FirstOrDefault();

        return levelData;
    }

    private static LOK1gameLogger GetLogger()
    {
        return App.Loggers.GetLogger(ELoggerGroup.LevelManager);
    }

    #region cmd

    [ConsoleCommand("restart_level", "Restarts current level")]
    private static void RestartLevelCommand()
    {
        GetLogger().Push("RestartLevel command executed");
        RestartLevel();
    }

    [ConsoleCommand("load_level", "Load level by MainScene name")]
    private static void LoadLevelCommand(string sceneName)
    {
        GetLogger().Push($"LoadLevel command executed for scene: {sceneName}");

        var levelData = GetLevelData(sceneName);
        if (levelData != null)
        {
            Coroutines.StartRoutine(LoadLevel(levelData));
        }
        else
        {
            GetLogger().PushError($"Level not found for scene: {sceneName}");
        }
    }

    #endregion
}
