using LOK1game.Game;
using LOK1game.UI;
using System;

namespace LOK1game
{
    public partial class App
    {
        #region facade
        public static SubtitleManager GetSubtitleManager() => ProjectContext.SubtitleManager;
        public static UILoadingScreen GetLoadingScreen() => ProjectContext.LoadingScreen;
        public static EGameStateId GetGameState() => ProjectContext.GameStateManager.CurrentGameStateId;
        public static GameStateManager GetGameStateManager() => ProjectContext.GameStateManager;


        #endregion

        #region loggers

        // General info loggers
        public static void GeneralLog(object message) => Loggers.GetLogger(ELoggerGroup.BaseInfo).Push(message);
        public static void GeneralLogWarning(object message) => Loggers.GetLogger(ELoggerGroup.BaseInfo).PushWarning(message);
        public static void GeneralLogError(object message) => Loggers.GetLogger(ELoggerGroup.BaseInfo).PushError(message);

        // Physics loggers
        public static void PhysicsLog(object message) => Loggers.GetLogger(ELoggerGroup.Physics).Push(message);
        public static void PhysicsLogWarning(object message) => Loggers.GetLogger(ELoggerGroup.Physics).PushWarning(message);
        public static void PhysicsLogError(object message) => Loggers.GetLogger(ELoggerGroup.Physics).PushError(message);

        // Network loggers
        public static void NetworkLog(object message) => Loggers.GetLogger(ELoggerGroup.Networking).Push(message);
        public static void NetworkLogWarning(object message) => Loggers.GetLogger(ELoggerGroup.Networking).PushWarning(message);
        public static void NetworkLogError(object message) => Loggers.GetLogger(ELoggerGroup.Networking).PushError(message);

        // Player loggers
        public static void PlayerLog(object message) => Loggers.GetLogger(ELoggerGroup.Player).Push(message);
        public static void PlayerLogWarning(object message) => Loggers.GetLogger(ELoggerGroup.Player).PushWarning(message);
        public static void PlayerLogError(object message) => Loggers.GetLogger(ELoggerGroup.Player).PushError(message);

        // AI loggers
        public static void AiLog(object message) => Loggers.GetLogger(ELoggerGroup.AI).Push(message);
        public static void AiLogWarning(object message) => Loggers.GetLogger(ELoggerGroup.AI).PushWarning(message);
        public static void AiLogError(object message) => Loggers.GetLogger(ELoggerGroup.AI).PushError(message);


        /// <summary>
        /// Pushes a log message to the application logger.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="sender">The object that generated the log message</param>
        /// <exception cref="ApplicationException">Thrown when no Application logger is found</exception>
        public static void ApplicationLog(object message, UnityEngine.Object sender = null)
        {
            if (Loggers.TryGetLogger(ELoggerGroup.Application, out var logger))
                logger.Push(message, sender);
            else
                throw new ApplicationException($"No Application logger container found. Caused by {sender}.//");
        }

        /// <summary>
        /// Pushes a log error message to the application logger.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="sender">The object that generated the log message</param>
        /// <exception cref="ApplicationException">Thrown when no Application logger is found</exception>
        public static void ApplicationLogError(object message, UnityEngine.Object sender = null)
        {
            if (Loggers.TryGetLogger(ELoggerGroup.Application, out var logger))
                logger.PushError(message, sender);
            else
                throw new ApplicationException($"No Application logger container found. Caused by {sender}.//");
        }

        #endregion
    }
}
