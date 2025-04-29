using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class DoubleJumpAbility : MonoBehaviour
    {


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                player.Movement.AllowDoubleJump();
            }
        }
    }
}
