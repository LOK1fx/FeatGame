using UnityEngine;

namespace LOK1game
{
    public class AISpawner : MonoBehaviour
    {
        [SerializeField] private Controller<PlayerDomain.Player> _aiController;
        [SerializeField] private PlayerDomain.Player _aiCharacterPrefab;

        private void Start()
        {
            var character = Pawn.Spawn(_aiCharacterPrefab, transform.position, transform.rotation, null) as PlayerDomain.Player;
            var controller = Instantiate(_aiController);

            controller.SetControlledPawn(character, false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one + Vector3.up);
        }
    }
}
