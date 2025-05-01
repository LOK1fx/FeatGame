using System;
using System.Collections.Generic;
using LOK1game.Game.Events;
using LOK1game.Utils;
using UnityEngine;

namespace LOK1game
{
    public sealed class App : MonoBehaviour
    {
        public static ProjectContext ProjectContext { get; private set; }
        public static Loggers Loggers { get; private set; }

        [SerializeField] private ProjectContext _projectContext = new ProjectContext();
        [SerializeField] private List<LoggerContainer> _loggerContainers = new List<LoggerContainer>();

        private const string APP_GAME_OBJECT_NAME = "[App]";

        #region Boot

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

        private void InitializeLoggers()
        {
            Loggers = new Loggers(_loggerContainers.ToArray());

            PushLogInfo("LOK1gameLogger initialized!");
        }

        [ContextMenu("Swap loggers")]
        private void SwapLoggers()
        {
            Loggers?.SwapLoggers(_loggerContainers.ToArray());
        }

        public static void PushLogInfo(object message, UnityEngine.Object sender = null)
        {
            if (Loggers.TryGetLogger(ELoggerGroup.Application, out var logger))
                logger.Push(message, sender);
            else
                throw new ApplicationException($"No Application logger container found. Caused by {sender}.//");
        }
    }
}