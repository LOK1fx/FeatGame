using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class SlideTrigger : MonoBehaviour
    {
        [SerializeField] private bool _makeSlide = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                if (_makeSlide)
                {
                    if (player.State.IsSliding == false)
                        player.Movement.StartSlide();
                }
                else
                {
                    if (player.State.IsSliding == true)
                        player.Movement.StopCrouch();
                }
            }
        }
    }
}
