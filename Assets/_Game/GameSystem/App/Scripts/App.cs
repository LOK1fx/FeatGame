using System;
using System.Collections.Generic;
using LOK1game.Game.Events;
using LOK1game.Utils;
using UnityEngine;

namespace LOK1game
{
    /// <summary>
    /// Main entry point of the application. Handles initialization of all game systems and provides global access to core services.
    /// This class is responsible for bootstrapping the application and managing its lifecycle.
    /// </summary>
    [RequireComponent(typeof(ApplicationUpdateManager))]
    public sealed class App : MonoBehaviour
    {
        /// <summary>
        /// Global access to the project context, containing game-specific settings and managers.
        /// </summary>
        public static ProjectContext ProjectContext { get; private set; }

        /// <summary>
        /// Global access to the logging system.
        /// </summary>
        public static Loggers Loggers { get; private set; }

        [SerializeField] private ProjectContext _projectContext = new ProjectContext();
        [SerializeField] private List<LoggerContainer> _loggerContainers = new List<LoggerContainer>();

        private const string APP_GAME_OBJECT_NAME = "[App]";

        #region Boot

        /// <summary>
        /// Initializes the application before the first scene is loaded.
        /// Creates and initializes the persistent App object.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Debug.Log("Starting application..");

            var app = Instantiate(Resources.Load<App>(APP_GAME_OBJECT_NAME));

            Debug.Log("Initializing persistanted App object..");

            if (app == null)
                throw new ApplicationException("Application prefab object not found! Put them into Resources folder.");

            app.name = APP_GAME_OBJECT_NAME;
            app.InitializeComponents();

            DontDestroyOnLoad(app.gameObject);

            // LOK1game logger initialized only in components
            // So we can use LOK1game logger only after bootstrap
            PushLogInfo("Application bootstrap is done successfuly!");
        }

        #endregion

        /// <summary>
        /// Gracefully shuts down the application, clearing all systems and events.
        /// </summary>
        /// <param name="exitCode">The exit code to return when the application closes</param>
        public static void Quit(int exitCode = 0)
        {
            Debug.Log($"Application started quit proccess with exitcode {exitCode}.");

            EventManager.Clear();
            Debug.Log("The EventManager has been cleared.");

            Application.Quit(exitCode);
            Debug.Log("Application quit.");

#if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false;
            
#endif
        }

        /// <summary>
        /// Initializes all core components of the application.
        /// Sets up loggers, and project context.
        /// </summary>
        private void InitializeComponents()
        {
            Debug.Log("Initializing components..");

            if (Application.isMobilePlatform)
                Application.targetFrameRate = Screen.currentResolution.refreshRate;

            InitializeLoggers();

            PushLogInfo("Initializing ProjectContext..");

            ProjectContext = _projectContext;
            ProjectContext.Initialize();

            PushLogInfo("ProjectContext initialized!");
            PushLogInfo("Application components initialized!");
        }

        /// <summary>
        /// Initializes the logging system with the configured logger containers.
        /// </summary>
        private void InitializeLoggers()
        {
            Loggers = new Loggers(_loggerContainers.ToArray());

            PushLogInfo("LOK1gameLogger initialized!");
        }

        /// <summary>
        /// Swaps the current logger configuration with a new one.
        /// Available in the Unity Editor context menu.
        /// </summary>
        [ContextMenu("Swap loggers")]
        private void SwapLoggers()
        {
            Loggers?.SwapLoggers(_loggerContainers.ToArray());
        }

        /// <summary>
        /// Pushes a log message to the application logger.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="sender">The object that generated the log message</param>
        /// <exception cref="ApplicationException">Thrown when no Application logger is found</exception>
        public static void PushLogInfo(object message, UnityEngine.Object sender = null)
        {
            if (Loggers.TryGetLogger(ELoggerGroup.Application, out var logger))
                logger.Push(message, sender);
            else
                throw new ApplicationException($"No Application logger container found. Caused by {sender}.//");
        }
    }
}