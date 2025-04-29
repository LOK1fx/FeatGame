using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class TeleportPlayerTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 _teleportPosition;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                player.Movement.StopSprint();
                player.Movement.StopCrouch();
                player.Movement.Rigidbody.linearVelocity = Vector3.zero;
                player.transform.position = _teleportPosition;
                player.Camera.SetFov(90f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _teleportPosition);

            Gizmos.DrawWireCube(_teleportPosition + Vector3.up, new Vector3(1f, 2f, 1f));
        }
    }
}
