using UnityEngine;

namespace LOK1game.World
{
    public class DefaultGameWorld : GameWorld
    {
        public bool DoubleJumpAllowrd => _doubleJumpAllowed;

        [SerializeField] private bool _doubleJumpAllowed = true;

        protected override void Initialize()
        {
            
        }
    }
}