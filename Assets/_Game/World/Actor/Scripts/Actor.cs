using LOK1game.Utils;
using LOK1game.World;
using UnityEngine;

namespace LOK1game
{
    public abstract class Actor : MonoBehaviour
    {
        public GameWorld CurrentWorld { get; private set; }


        public void PassWorld<T>(T world) where T : GameWorld
        {
            CurrentWorld = world;
        }

        protected virtual void SubscribeToEvents()
        {

        }

        protected virtual void UnsubscribeFromEvents()
        {

        }

        protected ProjectContext GetProjectContext()
        {
            return App.ProjectContext;
        }

        #region Loggers

        public LOK1gameLogger GetPlayerLogger()
        {
            return GetLoggers().GetLogger(ELoggerGroup.Player);
        }

        public LOK1gameLogger GetEnemiesLogger()
        {
            return GetLoggers().GetLogger(ELoggerGroup.Enemies);
        }

        public LOK1gameLogger GetAILogger()
        {
            return GetLoggers().GetLogger(ELoggerGroup.AI);
        }

        #endregion

        protected Loggers GetLoggers()
        {
            return App.Loggers;
        }
    }
}