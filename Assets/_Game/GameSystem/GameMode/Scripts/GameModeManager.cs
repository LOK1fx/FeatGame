using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOK1game.Tools;
using System.Linq;
using System;
using LOK1game.Utility;
using LOK1game.Utils;

namespace LOK1game.Game
{
    public struct GameModeContainer
    {
        public EGameModeId Id;
        public IGameMode GameMode;

        public GameModeContainer(EGameModeId id, IGameMode gameMode)
        {
            Id = id;
            GameMode = gameMode;
        }
    }

    public sealed class GameModeManager
    {
        public IGameMode CurrentGameMode { get; private set; }
        public EGameModeId CurrentGameModeId
        {
            get
            {
                return CurrentGameMode.Id;
            }
        }

        private readonly List<GameModeContainer> _gameModes = new List<GameModeContainer>();
        private bool _isSwithing;

        public void AddGameMode(EGameModeId id, IGameMode mode)
        {
            _gameModes.Add(new GameModeContainer(id, mode));
        }

        public void SetGameMode(EGameModeId id)
        {
            var gamemode = GetGameMode(id);

            Coroutines.StartRoutine(SwitchGameModeRoutine(gamemode));
        }

        public IEnumerator SwitchGameModeRoutine(EGameModeId id)
        {
            yield return SwitchGameModeRoutine(GetGameMode(id));
        }

        private IEnumerator SwitchGameModeRoutine(IGameMode gameMode)
        {
            if (CurrentGameMode != null)
                GetLogger().Push($"start switching {CurrentGameModeId} -> {gameMode.Id}");
            else
                GetLogger().Push($"start switching Nothing -> {gameMode.Id}");

            yield return new WaitUntil(() => !_isSwithing);

            _isSwithing = true;

            if (CurrentGameMode != null)
            {
                GetLogger().Push($"{CurrentGameModeId} {nameof(CurrentGameMode.OnEnd)}..");
                
                yield return CurrentGameMode.OnEnd();

                GetLogger().Push($"{CurrentGameModeId} ended");
            }

            GetLogger().Push($"{gameMode.Id} {nameof(gameMode.OnStart)}..");
            yield return gameMode.OnStart();
            
            GetLogger().Push($"{gameMode.Id} started");

            CurrentGameMode = gameMode;
            _isSwithing = false;

            GetLogger().Push($"Completed.");
        }

        private IGameMode GetGameMode(EGameModeId id)
        {
            var gameMode = _gameModes
                .Where(gamemode => gamemode.Id == id)
                .FirstOrDefault().GameMode;

            if (gameMode == null)
                throw new NullReferenceException($"{id} doesn't exist in ProjectContext GameModes list!");

            return gameMode;
        }

        private LOK1gameLogger GetLogger()
        {
            return App.Loggers.GetLogger(ELoggerGroup.GameModeManager);
        }

        [ConsoleCommand("list_game_modes", "Lists all game modes avaible into console")]
        private static void PrintGameModesListToConsole()
        {
            foreach (var gameMode in App.ProjectContext.GameModeManager._gameModes)
            {
                Debug.Log($"{Convert.ToInt32(gameMode.Id)} - {gameMode.Id}");
            }
        }

        [ConsoleCommand("current_game_mode", "Print CurrentGameMode to console")]
        private static void PrintCurrentGameModeToConsole()
        {
            Debug.Log(App.ProjectContext.GameModeManager.CurrentGameMode.ToString());
        }
    }
}