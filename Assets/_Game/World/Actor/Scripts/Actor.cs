using LOK1game.Utils;
using LOK1game.World;
using System.Collections;
using UnityEngine;

namespace LOK1game
{
    /// <summary>
    /// Base class for any game object placed in the scene with a world.
    /// Provides core functionality for game objects including world management, update cycle, and logging.
    /// </summary>
    public abstract class Actor : MonoBehaviour, IApplicationUpdatable, IDestroyableActor
    {
        /// <summary>
        /// The world instance this actor belongs to.
        /// </summary>
        public GameWorld CurrentWorld { get; private set; }
        public UniqueActorId UniqueId { get; private set; }

        protected virtual void OnEnable()
        {
            ApplicationUpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            ApplicationUpdateManager.Unregister(this);
        }

        public virtual void ApplyPitch(float angle)
        {
            var initalRot = transform.rotation.eulerAngles;
            transform.rotation.eulerAngles.Set(angle, initalRot.y, initalRot.z);
        }

        public virtual void ApplyYaw(float angle)
        {
            var initalRot = transform.rotation.eulerAngles;
            transform.rotation.eulerAngles.Set(initalRot.x, angle, initalRot.z);
        }

        public virtual void ApplyRoll(float angle)
        {
            var initalRot = transform.rotation.eulerAngles;
            transform.rotation.eulerAngles.Set(initalRot.x, initalRot.y, angle);
        }

        public virtual void ApplyRotation(Vector3 eulerAngles)
        {
            ApplyPitch(eulerAngles.x);
            ApplyYaw(eulerAngles.y);
            ApplyRoll(eulerAngles.z);
        }

        /// <summary>
        /// Assigns a world instance to this actor.
        /// </summary>
        /// <typeparam name="T">Type of the world</typeparam>
        /// <param name="world">The world instance to assign</param>
        public void PassWorld<T>(T world) where T : GameWorld
        {
            CurrentWorld = world;
        }

        public bool TryGetUniqueId(out UniqueActorId actorId)
        {
            if (UniqueId != null)
            {
                actorId = UniqueId;
                return true;
            }    

            if (TryGetComponent<UniqueActorId>(out var id))
            {
                UniqueId = id;
                actorId = id;
                return true;
            }
            
            actorId = null;
            return false;
        }

        /// <summary>
        /// Called every application update cycle. Override this method to implement custom update logic.
        /// </summary>
        public virtual void ApplicationUpdate()
        {
            
        }

        /// <summary>
        /// Called to subscribe to any events. Override this method to implement custom event subscriptions.
        /// </summary>
        protected virtual void SubscribeToEvents()
        {

        }

        /// <summary>
        /// Called to unsubscribe from any events. Override this method to implement custom event unsubscriptions.
        /// </summary>
        protected virtual void UnsubscribeFromEvents()
        {

        }

        /// <summary>
        /// Gets the current project context.
        /// </summary>
        /// <returns>The project context instance</returns>
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

        protected LOK1gameLogger GetLogger(ELoggerGroup loggerGroup)
        {
            return GetLoggers().GetLogger(loggerGroup);
        }

        public virtual IEnumerator OnActorDestroy()
        {
            yield return null;
        }
    }
}