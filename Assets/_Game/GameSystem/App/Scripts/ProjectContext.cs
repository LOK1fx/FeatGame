using LOK1game.Game;
using System;
using System.Collections.Generic;
using LOK1game.Game.Events;
using UnityEngine;
using LOK1game.UI;

namespace LOK1game
{
    /// <summary>
    /// Container for game-specific settings and managers. Acts as a central hub for accessing game systems and configurations.
    /// Initializes and manages game modes, game states, and development tools.
    /// </summary>
    [Serializable]
    public sealed class ProjectContext : Context
    {
        public LevelManager LevelManager => _levelManager;
        [SerializeField] private LevelManager _levelManager = new();

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

        [Header("Subtitles")]
        public SubtitleManager SubtitleManager { get; private set; }
        [SerializeField] private SubtitleManager _subtitleManagerPrefab;

        [Header("Loading screen")]
        public UILoadingScreen LoadingScreen { get; private set; }
        [SerializeField] private UILoadingScreen _loadingScreenPrefab;

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
            var loadingScreen = CreateLoadingScreen();
            LevelManager.Initialize(loadingScreen);

            InitializeGameModeManager();
            GameStateManager = new GameStateManager();

            InitializeDevCard();

            LocalisationSystem.Init();
            InitializeSubtitleManager();

            BroadcastProjectContextInitializedEvent();
            App.Loggers.GetLogger(ELoggerGroup.BaseInfo).Push("Project context initalized!");
        }

        private void InitializeGameModeManager()
        {
            _gameModeManager = new GameModeManager();
            var gameModesParent = new GameObject("[GAME_MODES]");
            GameObject.DontDestroyOnLoad(gameModesParent);

            foreach (var gameModePrefab in _gameModes)
            {
                var gameModeInstance = GameObject.Instantiate(gameModePrefab, gameModesParent.transform);
                
                if (gameModeInstance.TryGetComponent<IGameMode>(out var gameMode))
                {
                    gameModeInstance.name = gameModePrefab.name;
                    _gameModeManager.AddGameMode(gameMode);
                }
                else
                {
                    App.PushLogError($"{gameModePrefab.name} doesn't have {nameof(IGameMode)} interface! " +
                                     $"Ensure that prefab has one.");
                    GameObject.Destroy(gameModeInstance);
                }
            }
        }

        private void BroadcastProjectContextInitializedEvent()
        {
            var evt = new OnProjectContextInitializedEvent(this);
            EventManager.Broadcast(evt);
        }

        private void InitializeDevCard()
        {
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

        private void InitializeSubtitleManager()
        {
            SubtitleManager = GameObject.Instantiate(_subtitleManagerPrefab);
            GameObject.DontDestroyOnLoad(SubtitleManager);
        }

        private UILoadingScreen CreateLoadingScreen()
        {
            LoadingScreen = GameObject.Instantiate(_loadingScreenPrefab);
            GameObject.DontDestroyOnLoad(LoadingScreen);
            LoadingScreen.Hide();

            return LoadingScreen;
        }
    }
}
