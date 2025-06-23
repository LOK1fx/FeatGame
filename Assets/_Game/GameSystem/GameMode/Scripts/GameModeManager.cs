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
    
    public sealed class GameModeManager
    {
        private struct GameModeContainer
        {
            public EGameModeId Id;
            public IGameMode GameMode;

            public GameModeContainer(EGameModeId id, IGameMode gameMode)
            {
                Id = id;
                GameMode = gameMode;
            }
        }

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

        public void AddGameMode(IGameMode gameMode)
        {
            if (_gameModes.Where(g => g.Id == gameMode.Id).Any())
            {
                GetLogger().PushWarning($"GameMode {gameMode.Id} is already registred. Skipping..");
                return;
            }

            _gameModes.Add(new GameModeContainer(gameMode.Id, gameMode));
        }

        public IEnumerator SwitchGameModeRoutine(EGameModeId id, bool force = false)
        {
            yield return SwitchGameModeRoutine(GetGameMode(id), force);
        }

        private IEnumerator SwitchGameModeRoutine(IGameMode gameMode, bool force = false)
        {
            if (_isSwithing)
            {
                GetLogger().Push($"Switching is already in progress, wait...");
                while (_isSwithing)
                {
                    yield return null;
                }
            }
            
            _isSwithing = true;

            if (CurrentGameMode != null)
            {
                if (CurrentGameModeId != gameMode.Id || force == true)
                {
                    GetLogger().Push($"start switching <b>{CurrentGameModeId}</b> -> <b>{gameMode.Id}</b>");
                }
                else
                {
                    GetLogger().Push("GameModes are same.");
                    _isSwithing = false;
                    yield break;
                }
            }
            else
                GetLogger().Push($"start switching <b>Nothing</b> -> <b>{gameMode.Id}</b>");

            if (CurrentGameMode != null)
            {
                GetLogger().Push($"<b>{CurrentGameModeId}</b> {nameof(CurrentGameMode.OnEnd)}..");
                
                yield return CurrentGameMode.OnEnd();

                GetLogger().Push($"<b>{CurrentGameModeId}</b> ended");
            }

            GetLogger().Push($"<b>{gameMode.Id}</b> {nameof(gameMode.OnStart)}..");
            yield return gameMode.OnStart();
            
            GetLogger().Push($"<b>{gameMode.Id}</b> started");

            CurrentGameMode = gameMode;
            _isSwithing = false;
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

        #region cmd

        [ConsoleCommand("gm_list", "[GameMode Manager] Lists all game modes avaible into console")]
        private static void PrintGameModesListToConsole()
        {
            foreach (var gameMode in App.ProjectContext.GameModeManager._gameModes)
            {
                Debug.Log($"{Convert.ToInt32(gameMode.Id)} - {gameMode.Id}");
            }
        }

        [ConsoleCommand("gm_current", "[GameMode Manager] Print CurrentGameMode to console")]
        private static void PrintCurrentGameModeToConsole()
        {
            Debug.Log(App.ProjectContext.GameModeManager.CurrentGameMode.ToString());
        }

        [ConsoleCommand("gm_set", "[GameMode Manager] Changes current GameMode by id")]
        private static void SetGameMode(int gameModeId)
        {
            try
            {
                Coroutines.StartRoutine(App.ProjectContext.GameModeManager.SwitchGameModeRoutine((EGameModeId)Convert.ToUInt16(gameModeId)));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        #endregion
    }
}