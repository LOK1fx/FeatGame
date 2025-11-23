using LOK1game.Game.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game.Game
{
    public enum EGameStateId
    {
        Gameplay,
        Paused
    }

    public class GameStateManager
    {
        public EGameStateId CurrentGameStateId { get; private set; }
        public IGameState CurrentGameState { get; private set; }

        public delegate void GameStateChangeHandler(EGameStateId newGameState);
        public event GameStateChangeHandler OnGameStateChanged;

        private readonly Dictionary<EGameStateId, IGameState> _gameStates = new()
        {
            { EGameStateId.Paused, new PauseGameState() },
            { EGameStateId.Gameplay, new GameplayGameState() },
        };

        public void SetState(EGameStateId gameState)
        {
            if (gameState == CurrentGameStateId)
                return;

            if (_gameStates.ContainsKey(gameState) == false)
                throw new ArgumentOutOfRangeException(nameof(gameState));

            CurrentGameState?.OnExit();
            CurrentGameState = _gameStates[gameState];
            CurrentGameState.OnEnter();

            CurrentGameStateId = gameState;

            var evt = new OnGameStateChangedEvent(CurrentGameStateId, gameState);
            EventManager.Broadcast(evt);

            OnGameStateChanged?.Invoke(CurrentGameStateId);
        }
    }

    public interface IGameState
    {
        public EGameStateId Id { get; }

        public abstract void OnEnter();
        public abstract void OnExit();
    }

    public class PauseGameState : IGameState
    {
        public EGameStateId Id => EGameStateId.Paused;

        public void OnEnter()
        {
            Cursor.lockState = Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void OnExit() { }
    }

    public class GameplayGameState : IGameState
    {
        public EGameStateId Id => EGameStateId.Gameplay;

        public void OnEnter()
        {
            Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnExit() { }
    }
}