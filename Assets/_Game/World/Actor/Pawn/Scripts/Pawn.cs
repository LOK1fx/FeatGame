using System;
using System.Linq;
using UnityEngine;

namespace LOK1game
{
    /// <summary>
    /// Represents a controllable game object that can be possessed by a Controller.
    /// Inherits from Actor and adds functionality for player control and input handling.
    /// </summary>
    public abstract class Pawn : Actor, IPawn
    {
        [Header(nameof(Pawn))]
        /// <summary>
        /// Indicates whether this pawn is controlled by the local player.
        /// </summary>
        public bool IsLocallyControlled { get; private set; } = false;

        private Controller<IPawn> _controller;

        /// <summary>
        /// The controller currently possessing this pawn.
        /// </summary>
        //public Controller<IPawn> Controller => _controller;


        /// <summary>
        /// The controller currently possessing this pawn.
        /// </summary>
        public Controller<Pawntype> GetController<Pawntype>() where Pawntype: IPawn => _controller as Controller<Pawntype>;

        /// <summary>
        /// Called when input is received from the controller.
        /// </summary>
        /// <param name="sender">The object that sent the input</param>
        public abstract void OnInput(object sender, PlayerCharacterInputContext inputContext);

        /// <summary>
        /// Called when a controller takes possession of this pawn.
        /// </summary>
        /// <param name="sender">The controller that is taking possession</param>
        public virtual void OnPocces<Pawntype>(Controller<Pawntype> sender, PlayerCharacterInputContext inputContext) where Pawntype : IPawn
        {
            _controller = sender as Controller<IPawn>;
        }

        /// <summary>
        /// Called when a controller releases possession of this pawn.
        /// </summary>
        public virtual void OnUnpocces(PlayerCharacterInputContext inputContext)
        {
            _controller = null;
        }

        public void SetLocal(bool isLocal)
        {
            IsLocallyControlled = isLocal;
        }

        /// <summary>
        /// Gets a random spawn position for this pawn.
        /// </summary>
        /// <returns>A random spawn position vector</returns>
        public static Vector3 GetRandomSpawnPosition()
        {
            return GetRandomSpawnPosition(true);
        }

        /// <summary>
        /// Gets a random spawn position for this pawn, with option to filter for player spawn points.
        /// </summary>
        /// <param name="playerFlag">If true, only returns positions from spawn points that allow players</param>
        /// <returns>A random spawn position vector</returns>
        public static Vector3 GetRandomSpawnPosition(bool playerFlag)
        {
            var spawnPoints = FindObjectsByType<CharacterSpawnPoint>(FindObjectsSortMode.InstanceID).ToList();

            if (playerFlag)
                spawnPoints.RemoveAll(point => point.AllowPlayer == false);
            
            if (spawnPoints.Count < 1)
                return Vector3.zero;
            
            var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            return spawnPoint.transform.position;
        }
    }
}