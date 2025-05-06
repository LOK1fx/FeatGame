using LOK1game.Game;
using System;
using System.Collections.Generic;
using LOK1game.Game.Events;
using UnityEngine;

namespace LOK1game
{
    /// <summary>
    /// Container for game-specific settings and managers. Acts as a central hub for accessing game systems and configurations.
    /// Initializes and manages game modes, game states, and development tools.
    /// </summary>
    [Serializable]
    public sealed class ProjectContext : Context
    {
        /// <summary>
        /// Manager responsible for handling different game modes and their transitions.
        /// </summary>
        public GameModeManager GameModeManager => _gameModeManager;

        /// <summary>
        /// Manager responsible for handling game states (e.g., gameplay, paused).
        /// </summary>
        public GameStateManager GameStateManager { get; private set; }

        /// <summary>
        /// The default game mode ID that should be used when starting the game.
        /// </summary>
        public EGameModeId StandardGameModeId => _standardGameModeId;
        
        [Header("GameModes")]
        [SerializeField] private GameModeManager _gameModeManager;
        [SerializeField] private EGameModeId _standardGameModeId;
        [Header("Attention! In list, use only \nobjects with GM_ prefix!")]
        [SerializeField] private List<GameObject> _gameModes = new List<GameObject>();

#if DEVELOPMENT_BUILD || UNITY_EDITOR

        [Space]
        [Header("Dev")]
        [SerializeField] private GameObject _devCardPrefab;

#endif

        /// <summary>
        /// Initializes the project context by setting up game state manager, game mode manager,
        /// and registering all available game modes. Also initializes development tools if in development mode.
        /// </summary>
        public override void Initialize()
        {
            GameStateManager = new GameStateManager();
            _gameModeManager = new GameModeManager();

            foreach (var gameModeObject in _gameModes)
            {
                var gameMode = gameModeObject.GetComponent<IGameMode>();
                
                _gameModeManager.AddGameMode(gameMode.Id, gameMode);
            }

            var evt = new OnProjectContextInitializedEvent(this);
            EventManager.Broadcast(evt);

            App.Loggers.GetLogger(ELoggerGroup.BaseInfo).Push("Project context initalized!");

#if UNITY_EDITOR

            if (UnityEditor.EditorUserBuildSettings.development)
            {
                var devCard = GameObject.Instantiate(_devCardPrefab);
                GameObject.DontDestroyOnLoad(devCard);
            }

#elif DEVELOPMENT_BUILD

            var devCard = GameObject.Instantiate(_devCardPrefab);
            GameObject.DontDestroyOnLoad(devCard);

#endif
        }
    }
}
