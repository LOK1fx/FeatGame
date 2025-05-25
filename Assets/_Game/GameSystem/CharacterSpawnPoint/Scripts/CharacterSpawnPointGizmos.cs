using UnityEngine;

namespace LOK1game.Editor
{
    [RequireComponent(typeof(CharacterSpawnPoint))]
    public class CharacterSpawnPointGizmos : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField] private Color _capsuleColor = Color.green;
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Vector3 _meshScale = Vector3.one;
        [SerializeField] private Vector3 _meshPositionOffset;
        [SerializeField] private Vector3 _meshRotationOffset = Vector3.zero;

        private CharacterSpawnPoint _spawn;

        private void Awake()
        {
            _spawn = GetComponent<CharacterSpawnPoint>();
        }

        private void OnDrawGizmos()
        {
            var rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            if (_mesh != null)
            {
                Gizmos.color = _capsuleColor;
                var meshMatrix = Matrix4x4.TRS(_meshPositionOffset, Quaternion.Euler(_meshRotationOffset), _meshScale);
                Gizmos.matrix = rotationMatrix * meshMatrix;
                Gizmos.DrawWireMesh(_mesh, Vector3.zero, Quaternion.identity, Vector3.one);
            }


            
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.up * (Constants.Gameplay.PLAYER_HEIGHT * 0.5f),
                new Vector3(1, Constants.Gameplay.PLAYER_HEIGHT, 1));
            Gizmos.matrix = Matrix4x4.identity;
        }

#endif
    }
}