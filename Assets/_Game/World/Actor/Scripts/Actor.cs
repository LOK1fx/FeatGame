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

        protected Loggers GetLoggers()
        {
            return App.Loggers;
        }
    }
}